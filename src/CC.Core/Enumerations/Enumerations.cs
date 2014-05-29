using System;
using CC.Core.Localization;

namespace CC.Core.Enumerations
{
    public class Enumerations{}

    [Serializable]
    public class ValidationRule : Enumeration
    {
        public static readonly ValidationRule Empty = new ValidationRule { IsActive = false, Key = "" };
        public static readonly ValidationRule Required = new ValidationRule { IsActive = true, Key = "required" };
        public static readonly ValidationRule FileRequired = new ValidationRule { IsActive = true, Key = "fileRequired" };
        public static readonly ValidationRule Digits = new ValidationRule { IsActive = true, Key = "digits" };
        public static readonly ValidationRule Range = new ValidationRule { IsActive = true, Key = "range" };
        public static readonly ValidationRule Number = new ValidationRule { IsActive = true, Key = "number" };
        public static readonly ValidationRule Email = new ValidationRule { IsActive = true, Key = "email" };
        public static readonly ValidationRule Url = new ValidationRule { IsActive = true, Key = "url" };
        public static readonly ValidationRule Date = new ValidationRule { IsActive = true, Key = "date" };
    }

    [Serializable]
    public class ExtraViewModelData : Enumeration
    {
        public static readonly ExtraViewModelData Empty = new ExtraViewModelData { IsActive = false, Key = "" };
        public static readonly ExtraViewModelData CssRules = new ExtraViewModelData { IsActive = true, Key = "class" };
        public static readonly ExtraViewModelData InputMasks = new ExtraViewModelData { IsActive = true, Key = "mask" };
    }
    [Serializable]
    public class Status : Enumeration
    {
        public static readonly Status Empty = new Status { IsActive = false, Key = "" };
        public static readonly Status Active = new Status { IsActive = true, Key = "Active" };
        public static readonly Status InActive = new Status { IsActive = true, Key = "InActive" };
    }

    [Serializable]
    public class RuleOperatorEnum : Enumeration
    {
        public static readonly RuleOperatorEnum Empty = new RuleOperatorEnum { IsActive = false, Key = "" };
        public static readonly RuleOperatorEnum And = new RuleOperatorEnum { IsActive = true, Key = "And", Value = "And" };
        public static readonly RuleOperatorEnum Or = new RuleOperatorEnum { IsActive = true, Key = "Or", Value = "Or" };
    }

    [Serializable]
    public class State : Enumeration
    {
        public static readonly State Empty = new State { IsActive = false, Key = "" };

        public static readonly State AL = new State { IsActive = true, Key = "AL" };
        public static readonly State AK = new State { IsActive = true, Key = "AK" };
        public static readonly State AZ = new State { IsActive = true, Key = "AZ" };
        public static readonly State AR = new State { IsActive = true, Key = "AR" };
        public static readonly State CA = new State { IsActive = true, Key = "CA" };
        public static readonly State CO = new State { IsActive = true, Key = "CO" };
        public static readonly State CT = new State { IsActive = true, Key = "CT" };
        public static readonly State DE = new State { IsActive = true, Key = "DE" };
        public static readonly State FL = new State { IsActive = true, Key = "FL" };
        public static readonly State GA = new State { IsActive = true, Key = "GA" };
        public static readonly State HI = new State { IsActive = true, Key = "HI" };
        public static readonly State ID = new State { IsActive = true, Key = "ID" };
        public static readonly State IL = new State { IsActive = true, Key = "IL" };
        public static readonly State IN = new State { IsActive = true, Key = "IN" };
        public static readonly State IA = new State { IsActive = true, Key = "IA" };
        public static readonly State KS = new State { IsActive = true, Key = "KS" };
        public static readonly State KY = new State { IsActive = true, Key = "KY" };
        public static readonly State LA = new State { IsActive = true, Key = "LA" };
        public static readonly State ME = new State { IsActive = true, Key = "ME" };
        public static readonly State MD = new State { IsActive = true, Key = "MD" };
        public static readonly State MA = new State { IsActive = true, Key = "MA" };
        public static readonly State MI = new State { IsActive = true, Key = "MI" };
        public static readonly State MN = new State { IsActive = true, Key = "MN" };
        public static readonly State MS = new State { IsActive = true, Key = "MS" };
        public static readonly State MO = new State { IsActive = true, Key = "MO" };
        public static readonly State MT = new State { IsActive = true, Key = "MT" };
        public static readonly State NE = new State { IsActive = true, Key = "NE" };
        public static readonly State NV = new State { IsActive = true, Key = "NV" };
        public static readonly State NH = new State { IsActive = true, Key = "NH" };
        public static readonly State NJ = new State { IsActive = true, Key = "NJ" };
        public static readonly State NM = new State { IsActive = true, Key = "NM" };
        public static readonly State NY = new State { IsActive = true, Key = "NY" };
        public static readonly State NC = new State { IsActive = true, Key = "NC" };
        public static readonly State ND = new State { IsActive = true, Key = "ND" };
        public static readonly State OH = new State { IsActive = true, Key = "OH" };
        public static readonly State OK = new State { IsActive = true, Key = "OK" };
        public static readonly State OR = new State { IsActive = true, Key = "OR" };
        public static readonly State PA = new State { IsActive = true, Key = "PA" };
        public static readonly State RI = new State { IsActive = true, Key = "RI", IsDefault = true};
        public static readonly State SC = new State { IsActive = true, Key = "SC" };
        public static readonly State SD = new State { IsActive = true, Key = "SD" };
        public static readonly State TN = new State { IsActive = true, Key = "TN" };
        public static readonly State TX = new State { IsActive = true, Key = "TX" };
        public static readonly State UT = new State { IsActive = true, Key = "UT" };
        public static readonly State VT = new State { IsActive = true, Key = "VT" };
        public static readonly State VA = new State { IsActive = true, Key = "VA" };
        public static readonly State WA = new State { IsActive = true, Key = "WA" };
        public static readonly State WV = new State { IsActive = true, Key = "WV" };
        public static readonly State WI = new State { IsActive = true, Key = "WI" };
        public static readonly State WY = new State { IsActive = true, Key = "WY" };
        public static readonly State AS = new State { IsActive = true, Key = "AS" };
        public static readonly State DC = new State { IsActive = true, Key = "DC" };
        public static readonly State FM = new State { IsActive = true, Key = "FM" };
        public static readonly State GU = new State { IsActive = true, Key = "GU" };
        public static readonly State MH = new State { IsActive = true, Key = "MH" };
        public static readonly State MP = new State { IsActive = true, Key = "MP" };
        public static readonly State PW = new State { IsActive = true, Key = "PW" };
        public static readonly State PR = new State { IsActive = true, Key = "PR" };
        public static readonly State VI = new State { IsActive = true, Key = "VI" };
        
    }

    [Serializable]
    public class Country : Enumeration
    {
        public static readonly Country Empty = new Country { IsActive = false, Key = "" };
        public static readonly Country USA = new Country { IsActive = true, Key = "USA" };
        public static readonly Country France = new Country { IsActive = true, Key = "France" };
    }

    [Serializable]
    public class AreaName : Enumeration
    {
        public static readonly AreaName Empty = new AreaName { IsActive = false, Key = "" };
        public static readonly AreaName Permissions = new AreaName { IsActive = false, Key = "Permissions" };
    }

    [Serializable]
    public class Minutes : Enumeration
    {
        public static readonly Minutes Empty = new Minutes { IsActive = false, Key = "" };
        public static readonly Minutes Zero = new Minutes { IsActive = true, Key = "00" };
        public static readonly Minutes Fifteen = new Minutes { IsActive = true, Key = "15" };
        public static readonly Minutes Thirty = new Minutes { IsActive = true, Key = "30" };
        public static readonly Minutes FortyFive = new Minutes { IsActive = true, Key = "45" };
    }

    [Serializable]
    public class Hours : Enumeration
    {
        public static readonly Hours Empty = new Hours {IsActive = false, Key = ""};
        public static readonly Hours One = new Hours { IsActive = true, Key = "1" };
        public static readonly Hours Two = new Hours { IsActive = true, Key = "2" };
        public static readonly Hours Three = new Hours { IsActive = true, Key = "3" };
        public static readonly Hours Four = new Hours { IsActive = true, Key = "4" };
        public static readonly Hours Five = new Hours { IsActive = true, Key = "5" };
        public static readonly Hours Six = new Hours { IsActive = true, Key = "6" };
        public static readonly Hours Seven = new Hours { IsActive = true, Key = "7" };
        public static readonly Hours Eight = new Hours { IsActive = true, Key = "8" };
        public static readonly Hours Nine = new Hours { IsActive = true, Key = "9" };
        public static readonly Hours Ten = new Hours { IsActive = true, Key = "10" };
        public static readonly Hours Eleven = new Hours { IsActive = true, Key = "11" };
        public static readonly Hours Twelve = new Hours { IsActive = true, Key = "12" };
    }

    [Serializable]
    public class Time : Enumeration
    {
        public static readonly Time TwelveAM = new Time { IsActive = true, Key = "12:00 AM", Index = 1};
        public static readonly Time TwelveFifteenAM = new Time { IsActive = true, Key = "12:15 AM", Index = 2 };
        public static readonly Time TwelveThirtyAM = new Time { IsActive = true, Key = "12:30 AM", Index = 3 };
        public static readonly Time TwelveFourtyFiveAM = new Time { IsActive = true, Key = "12:45 AM", Index = 4 };
        public static readonly Time OneAM = new Time { IsActive = true, Key = "1:00 AM", Index = 5 };
        public static readonly Time OneFifteenAM = new Time { IsActive = true, Key = "1:15 AM", Index = 6 };
        public static readonly Time OneThirtyAM = new Time { IsActive = true, Key = "1:30 AM", Index = 7 };
        public static readonly Time OneFourtyFiveAM = new Time { IsActive = true, Key = "1:45 AM", Index = 8 };
        public static readonly Time TwoAM = new Time { IsActive = true, Key = "2:00 AM", Index = 9 };
        public static readonly Time TwoFifteenAM = new Time { IsActive = true, Key = "2:15 AM", Index = 10 };
        public static readonly Time TwoThirtyAM = new Time { IsActive = true, Key = "2:30 AM", Index = 11 };
        public static readonly Time TwoFourtyFiveAM = new Time { IsActive = true, Key = "2:45 AM", Index = 12 };
        public static readonly Time ThreeAM = new Time { IsActive = true, Key = "3:00 AM", Index = 13 };
        public static readonly Time ThreeFifteenAM = new Time { IsActive = true, Key = "3:15 AM", Index = 14 };
        public static readonly Time ThreeThirtyAM = new Time { IsActive = true, Key = "3:30 AM", Index = 15 };
        public static readonly Time ThreeFourtyFiveAM = new Time { IsActive = true, Key = "3:45 AM", Index = 16 };
        public static readonly Time FourAM = new Time { IsActive = true, Key = "4:00 AM", Index = 17 };
        public static readonly Time FourFifteenAM = new Time { IsActive = true, Key = "4:15 AM", Index = 18 };
        public static readonly Time FourThirtyAM = new Time { IsActive = true, Key = "4:30 AM", Index = 19 };
        public static readonly Time FourFourtyFiveAM = new Time { IsActive = true, Key = "4:45 AM", Index = 20 };
        public static readonly Time FiveAM = new Time { IsActive = true, Key = "5:00 AM", Index = 21 };
        public static readonly Time FiveFifteenAM = new Time { IsActive = true, Key = "5:15 AM", Index = 22 };
        public static readonly Time FiveThirtyAM = new Time { IsActive = true, Key = "5:30 AM", Index = 23 };
        public static readonly Time FiveFourtyFiveAM = new Time { IsActive = true, Key = "5:45 AM", Index = 24 };
        public static readonly Time SixAM = new Time { IsActive = true, Key = "6:00 AM", Index = 25 };
        public static readonly Time SixFifteenAM = new Time { IsActive = true, Key = "6:15 AM", Index = 26 };
        public static readonly Time SixThirtyAM = new Time { IsActive = true, Key = "6:30 AM", Index = 27 };
        public static readonly Time SixFourtyFiveAM = new Time { IsActive = true, Key = "6:45 AM", Index = 28 };
        public static readonly Time SevenAM = new Time { IsActive = true, Key = "7:00 AM", Index = 29 };
        public static readonly Time SevenFifteenAM = new Time { IsActive = true, Key = "7:15 AM", Index = 30 };
        public static readonly Time SevenThirtyAM = new Time { IsActive = true, Key = "7:30 AM", Index = 31 };
        public static readonly Time SevenFourtyFiveAM = new Time { IsActive = true, Key = "7:45 AM", Index = 32 };
        public static readonly Time EightAM = new Time { IsActive = true, Key = "8:00 AM", Index = 33 };
        public static readonly Time EightFifteenAM = new Time { IsActive = true, Key = "8:15 AM", Index = 34 };
        public static readonly Time EightThirtyAM = new Time { IsActive = true, Key = "8:30 AM", Index = 35 };
        public static readonly Time EightFourtyFiveAM = new Time { IsActive = true, Key = "8:45 AM", Index = 36 };
        public static readonly Time NineAM = new Time { IsActive = true, Key = "9:00 AM", Index = 37 };
        public static readonly Time NineFifteenAM = new Time { IsActive = true, Key = "9:15 AM", Index = 38 };
        public static readonly Time NineThirtyAM = new Time { IsActive = true, Key = "9:30 AM", Index = 39 };
        public static readonly Time NineFourtyFiveAM = new Time { IsActive = true, Key = "9:45 AM", Index = 40 };
        public static readonly Time TenAM = new Time { IsActive = true, Key = "10:00 AM", Index = 41 };
        public static readonly Time TenFifteenAM = new Time { IsActive = true, Key = "10:15 AM", Index = 42 };
        public static readonly Time TenThirtyAM = new Time { IsActive = true, Key = "10:30 AM", Index = 43 };
        public static readonly Time TenFourtyFiveAM = new Time { IsActive = true, Key = "10:45 AM", Index = 44 };
        public static readonly Time ElevinAM = new Time { IsActive = true, Key = "11:00 AM", Index = 45 };
        public static readonly Time ElevinFifteenAM = new Time { IsActive = true, Key = "11:15 AM", Index = 46 };
        public static readonly Time ElevinThirtyAM = new Time { IsActive = true, Key = "11:30 AM", Index = 47 };
        public static readonly Time ElevinFourtyFiveAM = new Time { IsActive = true, Key = "11:45 AM", Index = 48 };

        public static readonly Time TwelvePM = new Time { IsActive = true, Key = "12:00 PM", Index = 49 };
        public static readonly Time TwelveFifteenPM = new Time { IsActive = true, Key = "12:15 PM", Index = 50 };
        public static readonly Time TwelveThirtyPM = new Time { IsActive = true, Key = "12:30 PM", Index = 51 };
        public static readonly Time TwelveFourtyFivePM = new Time { IsActive = true, Key = "12:45 PM", Index = 52 };
        public static readonly Time OnePM = new Time { IsActive = true, Key = "1:00 PM", Index = 53 };
        public static readonly Time OneFifteenPM = new Time { IsActive = true, Key = "1:15 PM", Index = 54 };
        public static readonly Time OneThirtyPM = new Time { IsActive = true, Key = "1:30 PM", Index = 55 };
        public static readonly Time OneFourtyFivePM = new Time { IsActive = true, Key = "1:45 PM", Index = 56 };
        public static readonly Time TwoPM = new Time { IsActive = true, Key = "2:00 PM", Index = 57 };
        public static readonly Time TwoFifteenPM = new Time { IsActive = true, Key = "2:15 PM", Index = 58 };
        public static readonly Time TwoThirtyPM = new Time { IsActive = true, Key = "2:30 PM", Index = 59 };
        public static readonly Time TwoFourtyFivePM = new Time { IsActive = true, Key = "2:45 PM", Index = 60 };
        public static readonly Time ThreePM = new Time { IsActive = true, Key = "3:00 PM", Index = 62 };
        public static readonly Time ThreeFifteenPM = new Time { IsActive = true, Key = "3:15 PM", Index = 62 };
        public static readonly Time ThreeThirtyPM = new Time { IsActive = true, Key = "3:30 PM", Index = 63 };
        public static readonly Time ThreeFourtyFivePM = new Time { IsActive = true, Key = "3:45 PM", Index = 64 };
        public static readonly Time FourPM = new Time { IsActive = true, Key = "4:00 PM", Index = 65 };
        public static readonly Time FourFifteenPM = new Time { IsActive = true, Key = "4:15 PM", Index = 66 };
        public static readonly Time FourThirtyPM = new Time { IsActive = true, Key = "4:30 PM", Index = 67 };
        public static readonly Time FourFourtyFivePM = new Time { IsActive = true, Key = "4:45 PM", Index = 68 };
        public static readonly Time FivePM = new Time { IsActive = true, Key = "5:00 PM", Index = 69 };
        public static readonly Time FiveFifteenPM = new Time { IsActive = true, Key = "5:15 PM", Index = 70 };
        public static readonly Time FiveThirtyPM = new Time { IsActive = true, Key = "5:30 PM", Index = 71 };
        public static readonly Time FiveFourtyFivePM = new Time { IsActive = true, Key = "5:45 PM", Index = 72 };
        public static readonly Time SixPM = new Time { IsActive = true, Key = "6:00 PM", Index = 73 };
        public static readonly Time SixFifteenPM = new Time { IsActive = true, Key = "6:15 PM", Index = 74 };
        public static readonly Time SixThirtyPM = new Time { IsActive = true, Key = "6:30 PM", Index = 75 };
        public static readonly Time SixFourtyFivePM = new Time { IsActive = true, Key = "6:45 PM", Index = 76 };
        public static readonly Time SevenPM = new Time { IsActive = true, Key = "7:00 PM", Index = 77 };
        public static readonly Time SevenFifteenPM = new Time { IsActive = true, Key = "7:15 PM", Index = 78 };
        public static readonly Time SevenThirtyPM = new Time { IsActive = true, Key = "7:30 PM", Index = 79 };
        public static readonly Time SevenFourtyFivePM = new Time { IsActive = true, Key = "7:45 PM", Index = 80 };
        public static readonly Time EightPM = new Time { IsActive = true, Key = "8:00 PM", Index = 81 };
        public static readonly Time EightFifteenPM = new Time { IsActive = true, Key = "8:15 PM", Index = 82 };
        public static readonly Time EightThirtyPM = new Time { IsActive = true, Key = "8:30 PM", Index = 83 };
        public static readonly Time EightFourtyFivePM = new Time { IsActive = true, Key = "8:45 PM", Index = 84 };
        public static readonly Time NinePM = new Time { IsActive = true, Key = "9:00 PM", Index = 85 };
        public static readonly Time NineFifteenPM = new Time { IsActive = true, Key = "9:15 PM", Index = 86 };
        public static readonly Time NineThirtyPM = new Time { IsActive = true, Key = "9:30 PM", Index = 87 };
        public static readonly Time NineFourtyFivePM = new Time { IsActive = true, Key = "9:45 PM", Index = 88 };
        public static readonly Time TenPM = new Time { IsActive = true, Key = "10:00 PM", Index = 89 };
        public static readonly Time TenFifteenPM = new Time { IsActive = true, Key = "10:15 PM", Index = 90 };
        public static readonly Time TenThirtyPM = new Time { IsActive = true, Key = "10:30 PM", Index = 91 };
        public static readonly Time TenFourtyFivePM = new Time { IsActive = true, Key = "10:45 PM", Index = 92 };
        public static readonly Time ElevinPM = new Time { IsActive = true, Key = "11:00 PM", Index = 93 };
        public static readonly Time ElevinFifteenPM = new Time { IsActive = true, Key = "11:15 PM", Index = 94 };
        public static readonly Time ElevinThirtyPM = new Time { IsActive = true, Key = "11:30 PM", Index = 95 };
        public static readonly Time ElevinFourtyFivePM = new Time { IsActive = true, Key = "11:45 PM", Index = 96 };
    
    }

    [Serializable]
    public class AMPM : Enumeration
    {
        public static readonly AMPM Empty = new AMPM { IsActive = false, Key = "" };
        public static readonly AMPM AM = new AMPM { IsActive = true, Key = "AM" };
        public static readonly AMPM PM = new AMPM { IsActive = true, Key = "PM" };
    }

    [Serializable]
    public class Selector : Enumeration
    {
        public static readonly Selector Class = new Selector { IsActive = true, Key = "Class" };
        public static readonly Selector Id = new Selector { IsActive = true, Key = "Id"};
        public static readonly Selector Name = new Selector { IsActive = true, Key = "Name" };
        public static readonly Selector AnchorText = new Selector { IsActive = true, Key = "AnchorText" };
    }



}