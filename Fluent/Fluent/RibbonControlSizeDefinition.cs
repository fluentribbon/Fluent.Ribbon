namespace Fluent
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using Fluent.Converters;

    [TypeConverter(typeof(SizeDefinitionConverter))]
    public struct RibbonControlSizeDefinition
    {
        private const int MaxSizeDefinitionParts = 3;

        public RibbonControlSizeDefinition(RibbonControlSize large, RibbonControlSize middle, RibbonControlSize small)
            : this()
        {
            this.Large = large;
            this.Middle = middle;
            this.Small = small;
        }

        public RibbonControlSizeDefinition(string sizeDefinition)
            : this()
        {
            if (string.IsNullOrEmpty(sizeDefinition))
            {
                this.Large = RibbonControlSize.Large;
                this.Middle = RibbonControlSize.Large;
                this.Small = RibbonControlSize.Large;
                return;
            }

            var splitted = sizeDefinition.Split(new[] { ' ', ',', ';', '-', '>' }, MaxSizeDefinitionParts, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (splitted.Count == 0)
            {
                this.Large = RibbonControlSize.Large;
                this.Middle = RibbonControlSize.Large;
                this.Small = RibbonControlSize.Large;
                return;
            }

            // Ensure that we got three sizes
            for (var i = splitted.Count; i < MaxSizeDefinitionParts; i++)
            {
                splitted.Add(splitted[splitted.Count - 1]);
            }

            this.Large = ToRibbonControlSize(splitted[0]);
            this.Middle = ToRibbonControlSize(splitted[1]);
            this.Small = ToRibbonControlSize(splitted[2]);
        }

        public RibbonControlSize Large { get; set; }

        public RibbonControlSize Middle { get; set; }

        public RibbonControlSize Small { get; set; }

        public static implicit operator RibbonControlSizeDefinition(string sizeDefinition)
        {
            return new RibbonControlSizeDefinition(sizeDefinition);
        }

        public static implicit operator string(RibbonControlSizeDefinition sizeDefinition)
        {
            return sizeDefinition.ToString();
        }

        public static RibbonControlSize ToRibbonControlSize(string ribbonControlSize)
        {
            RibbonControlSize result;

            return Enum.TryParse(ribbonControlSize, true, out result) 
                ? result 
                : RibbonControlSize.Large;
        }

        public RibbonControlSize GetSize(RibbonGroupBoxState ribbonGroupBoxState)
        {
            switch (ribbonGroupBoxState)
            {
                case RibbonGroupBoxState.Large:
                    return this.Large;

                case RibbonGroupBoxState.Middle:
                    return this.Middle;

                case RibbonGroupBoxState.Small:
                    return this.Small;

                case RibbonGroupBoxState.Collapsed:
                case RibbonGroupBoxState.QuickAccess:
                    return this.Large;

                default:
                    return RibbonControlSize.Large;
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} {1} {2}", this.Large, this.Middle, this.Small);
        }
    }
}