using Lunch.Kiosk.Services.Interfaces;

namespace Lunch.Kiosk
{
    public partial class Form1 : Form
    {
        private readonly ICardService _cardService;

        public Form1(ICardService cardService)
        {
            InitializeComponent();
            _cardService = cardService;

            CardIdTxt.Multiline = true;
            CardIdTxt.AcceptsReturn = false;
            CardIdTxt.ScrollBars = ScrollBars.None;
            CardIdTxt.WordWrap = false;
            CardIdTxt.ShortcutsEnabled = false;
            CardIdTxt.Height = 44;
            CardIdTxt.TextAlign = HorizontalAlignment.Center;
            CardIdTxt.KeyDown += CardIdTxt_KeyDown!;

            InfoLbl.AutoSize = true;
            InfoLbl.AutoEllipsis = false;
            InfoLbl.TextAlign = ContentAlignment.MiddleCenter;

            Shown += (_, _) => CardIdTxt.Focus();
        }

        private async void CardIdTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                await SubmitScanAsync();
            }
        }

        private async Task SubmitScanAsync()
        {
            var cardId = CardIdTxt.Text.Trim();
            if (string.IsNullOrWhiteSpace(cardId)) return;

            SetInfo("Ověřuji…", Color.DimGray);

            var res = await _cardService.CheckCardAsync(cardId);

            SetInfo(res.Message, res.Success ? Color.LimeGreen : Color.OrangeRed);

            CardIdTxt.Clear();
            CardIdTxt.Focus();
        }

        private void SetInfo(string text, Color color)
        {
            InfoLbl.ForeColor = color;
            var maxW = Math.Max(200, (int)(ClientSize.Width * 0.8));
            InfoLbl.MaximumSize = new Size(maxW, 0);

            InfoLbl.Text = text;

            CenterUI();
        }

        private void CenterUI()
        {
            var spacing = 12;

            InfoLbl.MaximumSize = new Size((int)(ClientSize.Width * 0.8), 0);
            CardIdTxt.Width = Math.Min(420, (int)(ClientSize.Width * 0.6));

            int totalH = InfoLbl.Height + spacing + CardIdTxt.Height;
            int y = (ClientSize.Height - totalH) / 2;

            InfoLbl.Left = (ClientSize.Width - InfoLbl.Width) / 2;
            InfoLbl.Top = y;

            CardIdTxt.Left = (ClientSize.Width - CardIdTxt.Width) / 2;
            CardIdTxt.Top = y + InfoLbl.Height + spacing;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            SetInfo("Kontroluji připojení…", Color.DimGray);
            CenterUI();
            BeginInvoke(async () => await UpdateOnlineStatusAsync());
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CenterUI();
        }

        private async Task UpdateOnlineStatusAsync()
        {
            var online = await _cardService.PingAsync();
            SetInfo(online ? "Přiložte čip" : "Offline – API nedostupné",
                    online ? Color.DimGray : Color.OrangeRed);
            CardIdTxt.Focus();
        }
    }
}
