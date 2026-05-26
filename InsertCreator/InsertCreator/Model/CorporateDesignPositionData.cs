using Newtonsoft.Json;
using System.Drawing;

namespace HgSoftware.InsertCreator.Model
{
    public class CorporateDesignPositionData : IPositionData
    {
        #region Private Fields

        [JsonProperty("CorporateDesign Bar Color")]
        private int _barColorArgb;

        [JsonProperty("CorporateDesign Cornerbug Position")]
        private Point _cornerbugPosition = new Point();

        [JsonProperty("CorporateDesign Cornerbug Size")]
        private int _sizeCornerbug;

        [JsonProperty("CorporateDesign Text Color")]
        private int _textColorArgb;

        [JsonProperty("CorporateDesign White Stripe Color")]
        private int _stripeColorArgb;

        [JsonProperty("CorporateDesign First Bar Position")]
        private Point _firstBarPosition = new Point();

        [JsonProperty("CorporateDesign First Bar Size")]
        private Size _firstBarSize = new Size();

        [JsonProperty("CorporateDesign First Text Position")]
        private Point _firstTextPosition = new Point();

        [JsonProperty("CorporateDesign First Text Font")]
        private Font _firstTextFont;

        [JsonProperty("CorporateDesign Second Bar Position")]
        private Point _secondBarPosition = new Point();

        [JsonProperty("CorporateDesign Second Bar Size")]
        private Size _secondBarSize = new Size();

        [JsonProperty("CorporateDesign Second Text Position")]
        private Point _secondTextPosition = new Point();

        [JsonProperty("CorporateDesign Second Text Font")]
        private Font _secondTextFont;

        [JsonProperty("CorporateDesign White Stripe Position")]
        private Point _whiteStripePosition = new Point();

        [JsonProperty("CorporateDesign White Stripe Width")]
        private int _whiteStripeWidth;

        #endregion Private Fields

        #region Public Constructors

        public CorporateDesignPositionData()
        {
            _cornerbugPosition.X = 100;
            _cornerbugPosition.Y = 100;
            _sizeCornerbug = 100;

            _firstBarPosition.X = 100;
            _firstBarPosition.Y = 805;
            _firstBarSize.Width = 1200;
            _firstBarSize.Height = 80;

            _secondBarPosition.X = 100;
            _secondBarPosition.Y = 895;
            _secondBarSize.Width = 960;
            _secondBarSize.Height = 80;

            _firstTextPosition.X = 135;
            _firstTextPosition.Y = 805;
            _secondTextPosition.X = 135;
            _secondTextPosition.Y = 905;

            _whiteStripePosition.X = 100;
            _whiteStripePosition.Y = 805;
            _whiteStripeWidth = 6;

            _firstTextFont = new Font("Arial", 60, FontStyle.Bold, GraphicsUnit.Pixel);
            _secondTextFont = new Font("Arial", 52, FontStyle.Regular, GraphicsUnit.Pixel);

            _barColorArgb = ColorTranslator.FromHtml("#2786cf").ToArgb();
            _textColorArgb = Color.White.ToArgb();
            _stripeColorArgb = Color.White.ToArgb();
        }

        #endregion Public Constructors

        #region Public Properties

        [JsonIgnore]
        public Color BarColor
        {
            get { return Color.FromArgb(_barColorArgb); }
        }

        [JsonIgnore]
        public Point FirstBarPosition
        {
            get { return _firstBarPosition; }
        }

        [JsonIgnore]
        public Size FirstBarSize
        {
            get { return _firstBarSize; }
        }

        [JsonIgnore]
        public Point FirstTextPosition
        {
            get { return _firstTextPosition; }
        }

        [JsonIgnore]
        public Font FirstTextFont
        {
            get { return _firstTextFont; }
        }

        [JsonIgnore]
        public Point SecondBarPosition
        {
            get { return _secondBarPosition; }
        }

        [JsonIgnore]
        public Size SecondBarSize
        {
            get { return _secondBarSize; }
        }

        [JsonIgnore]
        public Point SecondTextPosition
        {
            get { return _secondTextPosition; }
        }

        [JsonIgnore]
        public Font SecondTextFont
        {
            get { return _secondTextFont; }
        }

        [JsonIgnore]
        public Color StripeColor
        {
            get { return Color.FromArgb(_stripeColorArgb); }
        }

        [JsonIgnore]
        public Point WhiteStripePosition
        {
            get { return _whiteStripePosition; }
        }

        [JsonIgnore]
        public int WhiteStripeWidth
        {
            get { return _whiteStripeWidth; }
        }

        [JsonIgnore]
        public Color TextColor
        {
            get { return Color.FromArgb(_textColorArgb); }
        }

        [JsonIgnore]
        public Point RectanglePosition
        {
            get { return new Point(); }
        }

        [JsonIgnore]
        public Size SizeRectangle
        {
            get { return new Size(); }
        }

        [JsonIgnore]
        public int TransparencyRectangle
        {
            get { return 0; }
        }


        [JsonIgnore]
        public PointF CornerbugPosition
        {
            get { return _cornerbugPosition; }
        }

        [JsonIgnore]
        public int SizeCornerbug
        {
            get { return _sizeCornerbug; }
        }

        PointF IPositionData.LogoPosition => PointF.Empty;

        int IPositionData.SizeLogo => 0;

        #endregion Public Properties
    }
}
