using System;
using MethodFitness.Core.Localization;

namespace MethodFitness.Core.Enumerations
{
    public class Enumerations{}

    [Serializable]
    public class ValidationRule : Enumeration
    {
        public static readonly ValidationRule Empty = new ValidationRule { IsActive = false, Key = "" };
        public static readonly ValidationRule Required = new ValidationRule { IsActive = true, Key = "required" };
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
    public class SkillSessionAction : Enumeration
    {
        public static readonly SkillSessionAction Empty = new SkillSessionAction { IsActive = false, Key = "" };
        public static readonly SkillSessionAction Saved = new SkillSessionAction { IsActive = true, Key = "Saved", Value = "Saved" };
        public static readonly SkillSessionAction SelfAssessed = new SkillSessionAction { IsActive = true, Key = "Self Assessed", Value = "SelfAssessed" };
        public static readonly SkillSessionAction Precepted = new SkillSessionAction { IsActive = true, Key = "Precepted", Value = "Precepted" };
        public static readonly SkillSessionAction Finalized = new SkillSessionAction { IsActive = true, Key = "Finalized", Value = "Finalized" };
        public static readonly SkillSessionAction SelfAcknowledged = new SkillSessionAction { IsActive = true, Key = "Self Acknowledged", Value = "SelfAcknowledged" };
        public static readonly SkillSessionAction Completed = new SkillSessionAction { IsActive = true, Key = "Completed", Value = "Completed" };
        public static readonly SkillSessionAction Comment = new SkillSessionAction { IsActive = true, Key = "Comment", Value = "Comment" };
    }

    [Serializable]
    public class RuleOperatorEnum : Enumeration
    {
        public static readonly RuleOperatorEnum Empty = new RuleOperatorEnum { IsActive = false, Key = "" };
        public static readonly RuleOperatorEnum And = new RuleOperatorEnum { IsActive = true, Key = "And", Value = "And" };
        public static readonly RuleOperatorEnum Or = new RuleOperatorEnum { IsActive = true, Key = "Or", Value = "Or" };
    }

    [Serializable]
    public class SkillQuestionMode : Enumeration
    {
        public static readonly SkillQuestionMode Empty = new SkillQuestionMode { IsActive = false, Key = "" };
        public static readonly SkillQuestionMode Act = new SkillQuestionMode { IsActive = true, Key = "Act", Value = "VERIFY" };
        public static readonly SkillQuestionMode Acknowledge = new SkillQuestionMode { IsActive = true, Key = "Acknowledge", Value = "ACKNOWLEDGE" };
        public static readonly SkillQuestionMode ViewSelf = new SkillQuestionMode { IsActive = true, Key = "ViewSelf", Value = "VIEWSELF" };
        public static readonly SkillQuestionMode Preview = new SkillQuestionMode { IsActive = true, Key = "Preview", Value = "PREVIEW" };
    }

    [Serializable]
    public class FrequencyScale : Enumeration
    {
        public static readonly FrequencyScale FixedDate = new FrequencyScale { IsActive = true, Key = "FixedDate" };
        public static readonly FrequencyScale Sliding = new FrequencyScale { IsActive = true, Key = "Sliding" };
        public static readonly FrequencyScale OneTime = new FrequencyScale { IsActive = true, Key = "OneTime" };
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
        public static readonly State RI = new State { IsActive = true, Key = "RI" };
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
    public class DocumentFileType : Enumeration
    {
        public static readonly DocumentFileType Empty = new DocumentFileType { IsActive = false, Key = "" };
        public static readonly DocumentFileType Document = new DocumentFileType { IsActive = true, Key = "Document" };
        public static readonly DocumentFileType Image = new DocumentFileType { IsActive = true, Key = "Image"};
        public static readonly DocumentFileType Headshot = new DocumentFileType { IsActive = true, Key = "Headshot"};
    }

    [Serializable]
    public class AreaName : Enumeration
    {
        public static readonly AreaName Empty = new AreaName { IsActive = false, Key = "" };
        public static readonly AreaName Portfolio = new AreaName { IsActive = true, Key = "Portfolio" };
    }

    [Serializable]
    public class AssetTypeEnum : Enumeration
    {
        public static readonly AssetTypeEnum Empty = new AssetTypeEnum { IsActive = false, Key = "" };
        public static readonly AssetTypeEnum AcademicCourse = new AssetTypeEnum { IsActive = true, Key = "AcademicCourse" };
        public static readonly AssetTypeEnum ClinicalExperience = new AssetTypeEnum { IsActive = true, Key = "ClinicalExperience" };
        public static readonly AssetTypeEnum Committee = new AssetTypeEnum { IsActive = true, Key = "Committee" };
        public static readonly AssetTypeEnum CommunityService = new AssetTypeEnum { IsActive = true, Key = "CommunityService" };
        public static readonly AssetTypeEnum ConsultingActivity = new AssetTypeEnum { IsActive = true, Key = "ConsultingActivity" };
        public static readonly AssetTypeEnum ContinuingEducation = new AssetTypeEnum { IsActive = true, Key = "ContinuingEducation" };
        public static readonly AssetTypeEnum Contract = new AssetTypeEnum { IsActive = true, Key = "Contract" };
        public static readonly AssetTypeEnum CoverText  = new AssetTypeEnum { IsActive = true, Key = "CoverText" };
        public static readonly AssetTypeEnum EditorialBoard = new AssetTypeEnum { IsActive = true, Key = "EditorialBoard" };
        public static readonly AssetTypeEnum Education = new AssetTypeEnum { IsActive = true, Key = "Education" };
        public static readonly AssetTypeEnum Fellowship = new AssetTypeEnum { IsActive = true, Key = "Fellowship" };
        public static readonly AssetTypeEnum FundedActivity = new AssetTypeEnum { IsActive = true, Key = "FundedActivity" };
        public static readonly AssetTypeEnum Goal = new AssetTypeEnum { IsActive = true, Key = "Goal" };
        public static readonly AssetTypeEnum Grant = new AssetTypeEnum { IsActive = true, Key = "Grant" };
        public static readonly AssetTypeEnum HealthPolicy = new AssetTypeEnum { IsActive = true, Key = "HealthPolicy" };
        public static readonly AssetTypeEnum Honor = new AssetTypeEnum { IsActive = true, Key = "Honor" };
        public static readonly AssetTypeEnum Interview = new AssetTypeEnum { IsActive = true, Key = "Interview" };
        public static readonly AssetTypeEnum License = new AssetTypeEnum { IsActive = true, Key = "License" };
        public static readonly AssetTypeEnum Membership = new AssetTypeEnum { IsActive = true, Key = "Membership" };
        public static readonly AssetTypeEnum MilitaryService = new AssetTypeEnum { IsActive = true, Key = "MilitaryService" };
        public static readonly AssetTypeEnum Presentation = new AssetTypeEnum { IsActive = true, Key = "Presentation" };
        public static readonly AssetTypeEnum ProfessionalCertification = new AssetTypeEnum { IsActive = true, Key = "ProfessionalCertification" };
        public static readonly AssetTypeEnum Publication = new AssetTypeEnum { IsActive = true, Key = "Publication" };
        public static readonly AssetTypeEnum Reflection = new AssetTypeEnum { IsActive = true, Key = "Reflection" };
        public static readonly AssetTypeEnum Research = new AssetTypeEnum { IsActive = true, Key = "Research" };
        public static readonly AssetTypeEnum Review = new AssetTypeEnum { IsActive = true, Key = "Review" };
        public static readonly AssetTypeEnum Software = new AssetTypeEnum { IsActive = true, Key = "Software" };
        public static readonly AssetTypeEnum StudentActivity = new AssetTypeEnum { IsActive = true, Key = "StudentActivity" };
        public static readonly AssetTypeEnum TeachingExperience = new AssetTypeEnum { IsActive = true, Key = "TeachingExperience" };
        public static readonly AssetTypeEnum TechnicalCertification = new AssetTypeEnum { IsActive = true, Key = "TechnicalCertification" };
        public static readonly AssetTypeEnum Thesis = new AssetTypeEnum { IsActive = true, Key = "Thesis" };
        public static readonly AssetTypeEnum Training = new AssetTypeEnum { IsActive = true, Key = "Training" };
        public static readonly AssetTypeEnum WorkHistory = new AssetTypeEnum { IsActive = true, Key = "WorkHistory" };
    }

    [Serializable]
    public class ComplianceItemEnum : Enumeration
    {
        public static readonly ComplianceItemEnum Empty = new ComplianceItemEnum { IsActive = false, Key = "" };
        public static readonly ComplianceItemEnum License = new ComplianceItemEnum { IsActive = true, Key = "License" };
        public static readonly ComplianceItemEnum ProfessionalCertification = new ComplianceItemEnum { IsActive = true, Key = "Professional Certification" };
        public static readonly ComplianceItemEnum TechnicalCertification = new ComplianceItemEnum { IsActive = true, Key = "Technical Certification" };
    }

    [Serializable]
    public class YesNo : Enumeration
    {
        public static readonly YesNo Empty = new YesNo { IsActive = false, Key = "" };
        public static readonly YesNo Yes = new YesNo { IsActive = true, Key = "Yes" };
        public static readonly YesNo No = new YesNo { IsActive = true, Key = "No" };
    }

    [Serializable]
    public class DefaultPhotoUrl : Enumeration
    {
        public static readonly DefaultPhotoUrl HeadShotUrl = new DefaultPhotoUrl { IsActive = true, Key = "/content/images/avatar.jpg" };
    }

    [Serializable]
    public class UserType : Enumeration
    {
        //basic user type, used for user listings to identify a usertype (not permissions)
        public static readonly UserType Admin = new UserType { IsActive = false, Key = "Admin" };
        public static readonly UserType Manager = new UserType { IsActive = true, Key = "Manager" };
        public static readonly UserType User = new UserType { IsActive = true, Key = "User" };
    }


}