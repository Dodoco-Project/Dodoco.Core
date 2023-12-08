namespace Dodoco.Core.Util.FileSystem {

    public static class DataUnitFormatter {

        private static readonly List<Tuple<string, string>> units = new List<Tuple<string, string>> {

            new Tuple<string, string>("B", "bytes"),
            new Tuple<string, string>("kB", "kilobytes"),
            new Tuple<string, string>("MB", "megabytes"),
            new Tuple<string, string>("GB", "gigabytes"),
            new Tuple<string, string>("TB", "terabytes")

        };

        public static string Format(double bytes) {

            return Format(bytes, DataUnitFormatterOption.USE_SYMBOL);

        }

        public static string Format(double bytes, DataUnitFormatterOption option) {

            double _base = 1000;
            double resultNumber = bytes;
            int index = 0;

            while (resultNumber >= _base) {
                
                resultNumber /= _base;
                index++;

            }

            string resultString = (Math.Truncate(resultNumber * 100) / 100).ToString(System.Globalization.CultureInfo.InvariantCulture);

            switch (option) {

                case DataUnitFormatterOption.USE_NONE:
                    break;
                case DataUnitFormatterOption.USE_SYMBOL:
                    resultString += $" {units[index].Item1}";
                    break;
                case DataUnitFormatterOption.USE_FULLNAME:
                    resultString += $" {units[index].Item2}";
                    break;

            }

            return resultString;

        }

    }

}