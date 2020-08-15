//	* ********************************************************************
//	*  © 2020 RazorSoft Media, DBA                                       *
//	*         Lone Star Logistics & Transport, LLC. All Rights Reserved  *
//	*         David Boarman                                              *
//	* ********************************************************************


namespace RazorSoft.Core.Configuration {
    public interface ISetting {

        string Name { get; }
        byte[] Value { get; }

        public interface ISetting<TValue> : ISetting {
            TValue SettingValue { get; }
        }
    }
}
