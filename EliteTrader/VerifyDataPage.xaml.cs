using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using EliteTrader.EliteOcr;
using EliteTrader.EliteOcr.Data;
using EliteTrader.EliteOcr.Enums;
using EliteTrader.EliteOcr.Interfaces;
using EliteTrader.EliteOcr.Logging;
using EliteTrader.ProgressReporting;
using ThruddClient;
using CommodityItem = EliteTrader.EliteOcr.CommodityItem;

namespace EliteTrader
{
    /// <summary>
    /// Interaction logic for VerifyDataPage.xaml
    /// </summary>
    public partial class VerifyDataPage : Window
    {
        public bool Success { get; private set; }

        private readonly ParsedScreenshot _parsedScreenshot;
        private readonly StationSearchResult _stationSearchResult;
        private readonly ThruddCredentials _credentials;
        private readonly VerifyDataViewModel _viewModel;

        private readonly ILogger _logger;

        public VerifyDataPage(ICommodityNameRepository commodityNameRepository, ParsedScreenshot parsedScreenshot, StationSearchResult stationSearchResult, ThruddCredentials credentials, ILogger logger)
        {
            Success = false;

            _logger = logger;

            _credentials = credentials;
            _viewModel = GetGridItems(commodityNameRepository, parsedScreenshot.Items, stationSearchResult.StationCommodities);
            _parsedScreenshot = parsedScreenshot;
            _stationSearchResult = stationSearchResult;

            InitializeComponent();

            StationNameLabel.Content = parsedScreenshot.StationName;

            if (_viewModel.InBoth.Count != 0)
            {
                InBothGrid.ItemsSource = _viewModel.InBoth;
            }
            else
            {
                Grid.RowDefinitions[0].MaxHeight = 0;
            }

            if (_viewModel.OnlyInScreenshot.Count != 0)
            {
                OnlyInScreenshotGrid.ItemsSource = _viewModel.OnlyInScreenshot;
            }
            else
            {
                Grid.RowDefinitions[1].MaxHeight = 0;
            }
        }

        private void DoConfirm(ThruddCredentials credentials)
        {
            ProgressDialogResult dialogResult = ProgressDialog.Execute(this, "Getting station information from Thrudd's website", () =>
            {
                //ProgressDialog.Current.ReportWithCancellationCheck(i * 5, "Executing step {0}/20...", i);
                Client client = new Client(_logger);
                try
                {
                    ProgressDialog.Current.ReportWithCancellationCheck(0, "Logging in");
                    client.Login(credentials);

                    List<InBothGridItem> inBoth = _viewModel.InBoth;
                    List<OnlyInScreenshotGridItem> onlyInScreenshot = _viewModel.OnlyInScreenshot;

                    int totalNumberOfItems = Math.Max(inBoth.Count + onlyInScreenshot.Count, 1);
                    decimal percentageIncrement = (decimal)90 / totalNumberOfItems;

                    int i = 0;
                    foreach (InBothGridItem item in inBoth)
                    {
                        if (item.DifferenceBuy == 0 && item.DifferenceSell == 0)
                        {
                            ProgressDialog.Current.ReportWithCancellationCheck(5 + (int)(percentageIncrement * i), "Confirming commodity ({0}) with Sell ({1}) and Buy ({2})", item.CommodityName, item.ScreenshotSell, item.ScreenshotBuy);
                            client.ConfirmCommodity(item.StationCommodityId, item.Category);
                        }
                        else
                        {
                            if (item.DifferenceBuy != 0)
                            {
                                ProgressDialog.Current.ReportWithCancellationCheck(5 + (int)(percentageIncrement * i), "Updating commodity ({0}) with Buy ({1})", item.CommodityName, item.ScreenshotBuy);
                                client.UpdateCommodity(item.StationCommodityId, EnumCommodityAction.Buy, item.ScreenshotBuy);
                            }
                            if (item.DifferenceSell != 0)
                            {
                                ProgressDialog.Current.ReportWithCancellationCheck(5 + (int)(percentageIncrement * i), "Updating commodity ({0}) with Sell ({1})", item.CommodityName, item.ScreenshotSell);
                                client.UpdateCommodity(item.StationCommodityId, EnumCommodityAction.Sell, item.ScreenshotSell);
                            }
                        }

                        ++i;
                    }

                    foreach (OnlyInScreenshotGridItem item in onlyInScreenshot)
                    {
                        ProgressDialog.Current.ReportWithCancellationCheck(5 + (int)(percentageIncrement * i), "Adding new commodity ({0}) to station with Sell ({1}) and Buy ({2})", item.CommodityName, item.Sell, item.Buy);
                        client.AddCommodity(_stationSearchResult.StationInfo.StationId, item.NameId, item.Buy, item.Sell, item.Category);

                        ++i;
                    }
                }
                finally
                {
                    ProgressDialog.Current.ReportWithCancellationCheck(95, "Logging out");
                    client.Logout();
                }

            }, new ProgressDialogSettings(true, false, false));

            if (dialogResult.OperationFailed)
            {
                throw new Exception("Exception during Thrudd website operations", dialogResult.Error);
            }
        }

        private void ConfirmButtonClick(object sender, RoutedEventArgs eventArgs)
        {
            try
            {
                DoConfirm(_credentials);

                Success = true;
                MessageBox.Show("Success");
                Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private VerifyDataViewModel GetGridItems(ICommodityNameRepository commodityNameRepository, List<CommodityItem> screenshotItems, List<StationCommoditiesData> existingItems)
        {
            //TODO Add support for rare commodities

            Dictionary<EnumCommodityItemName, StationCommoditiesData> existingItemsLookup =
                existingItems.Where(a => a.CommodityEnum != EnumCommodityItemName.Rare).ToDictionary(a => a.CommodityEnum);

            List<InBothGridItem> inBoth = new List<InBothGridItem>();
            List<OnlyInScreenshotGridItem> onlyInScreenshot = new List<OnlyInScreenshotGridItem>();

            foreach (CommodityItem screenshotItem in screenshotItems)
            {
                if (screenshotItem.Name == EnumCommodityItemName.Rare)
                {
                    continue;
                }

                string name = commodityNameRepository.Get(screenshotItem.Name);

                StationCommoditiesData existingItem;
                if (!existingItemsLookup.TryGetValue(screenshotItem.Name, out existingItem))
                {
                    OnlyInScreenshotGridItem p = new OnlyInScreenshotGridItem(screenshotItem.Name, CommodityItemHelper.GetCategory(screenshotItem.Name), name, screenshotItem.Sell ?? 0, screenshotItem.Buy ?? 0);

                    onlyInScreenshot.Add(p);

                    continue;
                }

                InBothGridItem p2 = new InBothGridItem(screenshotItem.Name, CommodityItemHelper.GetCategory(screenshotItem.Name), existingItem.Id, name, screenshotItem.Sell ?? 0, existingItem.Sell, screenshotItem.Buy ?? 0, existingItem.Buy);
                inBoth.Add(p2);
            }

            return new VerifyDataViewModel(inBoth, onlyInScreenshot);
        }
    }

    public class VerifyDataViewModel
    {
        public List<InBothGridItem> InBoth { get; private set; }
        public List<OnlyInScreenshotGridItem> OnlyInScreenshot { get; private set; }

        public VerifyDataViewModel(List<InBothGridItem> inBoth, List<OnlyInScreenshotGridItem> onlyInScreenshot)
        {
            InBoth = inBoth;
            OnlyInScreenshot = onlyInScreenshot;
        }
    }

    public class OnlyInScreenshotGridItem
    {
        public EnumCommodityItemName NameId { get; private set; }
        public EnumCommodityCategory Category { get; private set; }
        public string CommodityName { get; private set; }
        public int Sell { get; set; }
        public int Buy { get; set; }

        public OnlyInScreenshotGridItem(EnumCommodityItemName nameId, EnumCommodityCategory category, string commodityName, int sell, int buy)
        {
            NameId = nameId;
            Category = category;
            CommodityName = commodityName;
            Sell = sell;
            Buy = buy;
        }
    }

    public class InBothGridItem
    {
        public EnumCommodityItemName NameId { get; private set; }
        public EnumCommodityCategory Category { get; private set; }
        public int StationCommodityId { get; private set; }
        public string CommodityName { get; private set; }
        public int ScreenshotSell { get; set; }
        public int ExistingSell { get; private set; }
        public int ScreenshotBuy { get; set; }
        public int ExistingBuy { get; private set; }

        public string LargeChange
        {
            get
            {
                if (IsLargeChange())
                {
                    return "X";
                }
                return "";
            }
        }

        private bool IsLargeChange()
        {
            const decimal threshold = 0.15m;

            if ((ScreenshotSell == 0 && ExistingSell != 0) || (ScreenshotSell != 0 && ExistingSell == 0))
            {
                return true;
            }
            if ((ScreenshotBuy == 0 && ExistingBuy != 0) || (ScreenshotBuy != 0 && ExistingBuy == 0))
            {
                return true;
            }
            if (ScreenshotSell == 0 && ScreenshotBuy == 0)
            {
                return false;
            }

            if (DifferenceSell <= 10 && DifferenceBuy <= 10)
            {
                return false;
            }

            if (ExistingSell != 0)
            {
                decimal percentageSell = Math.Abs(((decimal) ScreenshotSell/ExistingSell) - 1);

                if (percentageSell > threshold)
                {
                    return true;
                }
            }

            if (ExistingBuy != 0)
            {
                decimal percentageBuy = Math.Abs(((decimal) ScreenshotBuy/ExistingBuy) - 1);

                if (percentageBuy > threshold)
                {
                    return true;
                }
            }

            return false;
        }

        public int DifferenceSell
        {
            get
            {
                return ScreenshotSell - ExistingSell;
            }
        }

        public int DifferenceBuy
        {
            get
            {
                return ScreenshotBuy - ExistingBuy;
            }
        }

        public InBothGridItem(EnumCommodityItemName nameId, EnumCommodityCategory category, int stationCommodityId, string commodityName, int screenshotSell, int existingSell, int screenshotBuy, int existingBuy)
        {
            NameId = nameId;
            Category = category;
            StationCommodityId = stationCommodityId;
            CommodityName = commodityName;
            ScreenshotSell = screenshotSell;
            ExistingSell = existingSell;
            ScreenshotBuy = screenshotBuy;
            ExistingBuy = existingBuy;
        }
    }
}
