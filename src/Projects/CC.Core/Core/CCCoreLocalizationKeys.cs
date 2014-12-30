using CC.Core.Core.Localization;

namespace CC.Core.Core
{
    public class CCCoreLocalizationKeys : StringToken
    {
        protected CCCoreLocalizationKeys(string key)
            : this(key, null)
        {
        }

        protected CCCoreLocalizationKeys(string key, string default_EN_US_Text)
            : base(key, default_EN_US_Text)
        {
        }

        public static readonly StringToken FIELD_REQUIRED = new CCCoreLocalizationKeys("FIELD_REQUIRED", "{0} Field is Required");
        public static readonly StringToken CONFIRMATION_PASSWORD_MUST_MATCH = new CCCoreLocalizationKeys("CONFIRMATION_PASSWORD_MUST_MATCH", "Confirmation password must match");
        public static readonly StringToken FILE_IS_REQUIRED = new CCCoreLocalizationKeys("FILE_IS_REQUIRED", "You must select a file");
        public static readonly StringToken VALID_EMAIL_FORMAT = new CCCoreLocalizationKeys("VALID_EMAIL_FORMAT", "{0} Must be a valid Email Address");
        public static readonly StringToken VALID_DATE_FORMAT = new CCCoreLocalizationKeys("VALID_DATE_FORMAT", "{0} Must be a valid Date");
        public static readonly StringToken VALID_RANGE = new CCCoreLocalizationKeys("VALID_RANGE", "{0} Must be between {1} and {2}");
        public static readonly StringToken FIELD_MUST_BE_NUMBER = new CCCoreLocalizationKeys("FIELD_MUST_BE_NUMBER", "{0} Field Requires a Number");
        public static readonly StringToken SITE_NAME = new CCCoreLocalizationKeys("SITE_NAME", "Know Your Turf");
        public static readonly StringToken SELECT_ITEM = new CCCoreLocalizationKeys("SELECT_ITEM", "-- Please Select --");
        public static readonly StringToken INVALID_USERASSISTANTITEM = new CCCoreLocalizationKeys("INVALID_USERASSISTANTITEM", "Some of these items have invalid information");
        public static readonly StringToken INSUFFICIENT_ITEM_INSTANCES = new CCCoreLocalizationKeys("INSUFFICIENT_ITEM_INSTANCES", "This item requires {0} instances, but only has {1} instances");
        public static readonly StringToken ACTOR_IS_NOT_THE_PERSON_BEING_TESTED = new CCCoreLocalizationKeys("ACTOR_IS_NOT_THE_PERSON_BEING_TESTED", "The Actor is not the person being tested.");
        public static readonly StringToken ACTOR_IS_THE_PERSON_BEING_TESTED = new CCCoreLocalizationKeys("ACTOR_IS_THE_PERSON_BEING_TESTED", "The Actor is the person being tested.");
        public static readonly StringToken THE_PERSON_BEING_TESTED_HAS_NOT_SUBMITTED_THIER_ANSWERS = new CCCoreLocalizationKeys("THE_PERSON_BEING_TESTED_HAS_NOT_SUBMITTED_THIER_ANSWERS", "The person being tested has not subitted thier answers.");
        public static readonly StringToken THE_PERSON_BEING_TESTED_HAS_SUBMITTED_THIER_ANSWERS = new CCCoreLocalizationKeys("THE_PERSON_BEING_TESTED_HAS_SUBMITTED_THIER_ANSWERS", "The person being tested has subitted thier answers.");
        public static readonly StringToken THE_CHECKLIST_HAS_NOT_BEEN_COMPLETED = new CCCoreLocalizationKeys("THE_CHECKLIST_HAS_NOT_BEEN_COMPLETED", "The Checklist has not been completed.");
        public static readonly StringToken THE_CHECKLIST_HAS_NOT_BEEN_FINIALIZED = new CCCoreLocalizationKeys("THE_CHECKLIST_HAS_NOT_BEEN_FINIALIZED", "The Checklist has not been finalized.");
        public static readonly StringToken THE_CHECKLIST_HAS_BEEN_FINIALIZED = new CCCoreLocalizationKeys("THE_CHECKLIST_HAS_BEEN_FINIALIZED", "The Checklist has been finalized.");
        public static readonly StringToken THE_CHECKLIST_HAS_BEEN_COMPLETED = new CCCoreLocalizationKeys("THE_CHECKLIST_HAS_BEEN_COMPLETED", "The Checklist has been completed.");
        public static readonly StringToken SUCCESSFUL_SAVE = new CCCoreLocalizationKeys("SUCCESSFUL_SAVE", "This operation has completed successfully");
        public static readonly StringToken DECISION_CRITICAL_EMAIL_CONFIRMATION = new CCCoreLocalizationKeys("DECISION_CRITICAL_EMAIL_CONFIRMATION", "Decision Critical Email Confirmation");
        public static readonly StringToken VALID_URL_FORMAT = new CCCoreLocalizationKeys("VALID_URL_FORMAT", "{0} Must be a valid Url");
        public static readonly StringToken INVALID_YEAR_MESSAGE = new CCCoreLocalizationKeys("INVALID_YEAR_MESSAGE", "Enter a Valid Year");
        public static readonly StringToken PRESENT = new CCCoreLocalizationKeys("PRESENT", "Present");
        public static readonly StringToken TRANSACTION_DECLINED = new CCCoreLocalizationKeys("TRANSACTION_DECLINED", "Transaction Declined");
        public static readonly StringToken TRANSACTION_SUCCESSFUL = new CCCoreLocalizationKeys("TRANSACTION_SUCCESSFUL", "Your transaction has been successfully Processed.  You will be recieving an Email at the address proviced shortly.  " +
                                                                                                                       "The email will contain a link that will direct you to your new account at Decision Critical!");
        public static readonly StringToken THANK_YOU_FOR_SUBSCRIBING = new CCCoreLocalizationKeys("THANK_YOU_FOR_SUBSCRIBING", "Thank you for joining Decision Critical!");


        public static readonly StringToken BUISNESS_RULE = new CCCoreLocalizationKeys("BUISNESS_RULE", "Business Rule");
        public static readonly StringToken YOU_CAN_NOT_CREATE_RETROACTIVE_APPOINTMENTS = new CCCoreLocalizationKeys("YOU_CAN_NOT_CREATE_RETROACTIVE_APPOINTMENTS", "You can not create retroactive Appointments.");

        public static readonly StringToken CLIENTS = new CCCoreLocalizationKeys("CLIENTS", "Clients");
        public static readonly StringToken SELECT_AT_LEAST_ONE_CLIENT = new CCCoreLocalizationKeys("SELECT_AT_LEAST_ONE_CLIENT", "You must select at least one Client.");
        public static readonly StringToken CLIENT_HAS_APPOINTMENTS_IN_FUTURE = new CCCoreLocalizationKeys("CLIENT_HAS_APPOINTMENTS_IN_FUTURE", "This Client has {0} Appointment(s) in the Future.");
        public static readonly StringToken TRAINER_HAS_APPOINTMENTS_IN_FUTURE = new CCCoreLocalizationKeys("TRAINER_HAS_APPOINTMENTS_IN_FUTURE", "This Trainer has {0} Appointment(s) in the Future.");

    }
}