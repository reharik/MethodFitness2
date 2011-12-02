using System;
using MethodFitness.Core.Localization;

namespace MethodFitness.Web
{
    public class WebLocalizationKeys: StringToken
    {
        protected WebLocalizationKeys(string key) : this(key, null)
        {
        }

        protected WebLocalizationKeys(string key, string default_EN_US_Text)
            : base(key, default_EN_US_Text)
        {
        }

        public static readonly StringToken EMPLOYEE_DASHBOARD = new WebLocalizationKeys("EMPLOYEE_DASHBOARD", "Employee Dashboard");
        public static readonly StringToken CALENDAR = new WebLocalizationKeys("CALENDAR", "Calendar");
        public static readonly StringToken TRAINERS = new WebLocalizationKeys("TRAINERS", "Trainers");
        public static readonly StringToken TRAINER = new WebLocalizationKeys("TRAINER", "Trainer");
        public static readonly StringToken NEW_TRAINER = new WebLocalizationKeys("NEW_TRAINER", "New Trainer");
        public static readonly StringToken TRAINER_INFORMATION = new WebLocalizationKeys("TRAINER_INFORMATION", "Trainer Information");






        public static readonly StringToken ADMIN_TOOLS = new WebLocalizationKeys("ADMIN_TOOLS", "Admin Tools");
        public static readonly StringToken REQUIRED = new WebLocalizationKeys("REQUIRED", "Required");
        public static readonly StringToken EMPTY_TOKEN = new WebLocalizationKeys("EMPTY_TOKEN", "");
        public static readonly StringToken SIGN_IN = new WebLocalizationKeys("SIGN_IN", "Sign In");
        public static readonly StringToken LOG_OUT = new WebLocalizationKeys("LOG_OUT", "Logout");
        public static readonly StringToken WELCOME = new WebLocalizationKeys("WELCOME", "Welcome ,");
        public static readonly StringToken INADMINMODE = new WebLocalizationKeys("INADMINMODE", "INADMINMODE");
        public static readonly StringToken USER_ROLES = new WebLocalizationKeys("USER_ROLES", "UserRoles");
        public static readonly StringToken ACCOUNT_LOGIN = new WebLocalizationKeys("ACCOUNT_LOGIN", "Account LogIn");
        public static readonly StringToken INVALID_USERNAME_OR_PASSWORD = new WebLocalizationKeys("INVALID_USERNAME_OR_PASSWORD", "Invalid Username or Password");
        public static readonly StringToken PLEASE_ENTER_YOUR_USERNAME_AND_PASSWORD_BELOW_KEY = new WebLocalizationKeys("PLEASE_ENTER_YOUR_USERNAME_AND_PASSWORD_BELOW_KEY", "Please enter your Username and Password below");
        public static readonly StringToken YOU_HAVE_BEEN_SENT_A_CONFIRMATION_EMAIL = new WebLocalizationKeys("YOU_HAVE_BEEN_SENT_A_CONFIRMATION_EMAIL", "Thank you! you have been sent a confirmation email.");
        public static readonly StringToken THANK_YOU_FOR_REGISTERING = new WebLocalizationKeys("THANK_YOU_FOR_REGISTERING", "Thank you for registering with MethodFitness!");
        public static readonly StringToken OR_CLICK_HERE_TO_REGISTER = new WebLocalizationKeys("OR_CLICK_HERE_TO_REGISTER", "Or click HERE to register!");
        public static readonly StringToken REGISTER_FOR_DECISION_CRITICAL = new WebLocalizationKeys("REGISTER_FOR_DECISION_CRITICAL", "Register For Decision Critical!");
        public static readonly StringToken ENTER_YOUR_EMAIL_ADDRESS_AND_WE_WILL_SEND_YOU_YOUR_INFORMATION = new WebLocalizationKeys("ENTER_YOUR_EMAIL_ADDRESS_AND_WE_WILL_SEND_YOU_YOUR_INFORMATION", "Enter your email address and we will send you your information");
        public static readonly StringToken UNABLE_TO_FIND_EMAIL_ADDRESS = new WebLocalizationKeys("UNABLE_TO_FIND_EMAIL_ADDRESS", "Unable to find the email address you entered");
        public static readonly StringToken YOUR_PASSWORD_ADDRESS_HAS_BEEN_SENT_TO_YOU = new WebLocalizationKeys("YOUR_PASSWORD_ADDRESS_HAS_BEEN_SENT_TO_YOU", "Your password has been emailed to you.");
        public static readonly StringToken DECISION_CRITICAL = new WebLocalizationKeys("DECISION_CRITICAL", "Decision Critical");
        public static readonly StringToken FORGOT_YOUR_PASSWORD = new WebLocalizationKeys("FORGOT_YOUR_PASSWORD", "Forgot your password?");
        public static readonly StringToken FORGOT_YOUR_PASSWORD_POPUP_TITLE = new WebLocalizationKeys("FORGOT_YOUR_PASSWORD_POPUP_TITLE", "Forgotten Password");
        public static readonly StringToken USER_PROFILE = new WebLocalizationKeys("USER_PROFILE", "User Profile");
        public static readonly StringToken MY_ACCOUNT= new WebLocalizationKeys("USER_PROFILE", "My Account");
        public static readonly StringToken PORTFOLIO_DASHBOARD= new WebLocalizationKeys("PORTFOLIO_DASHBOARD", "Portfolio Dashboard");
        public static readonly StringToken YOUR_FIRST_PORTFOLIO = new WebLocalizationKeys("YOUR_FIRST_PORTFOLIO", "Your First Portfolio");
        public static readonly StringToken PROFILE = new WebLocalizationKeys("PROFILE", "Profile");
        public static readonly StringToken HELP = new WebLocalizationKeys("HELP", "Help");

        public static readonly StringToken DASHBOARD = new WebLocalizationKeys("DASHBOARD", "Dashboard");
        public static readonly StringToken PORTFOLIOS = new WebLocalizationKeys("PORTFOLIOS", "Portfolios");

        public static readonly StringToken FIELD_REQUIRED = new WebLocalizationKeys("FIELD_REQUIRED", "{0} Field is Required");
        public static readonly StringToken USER_COMPLIANCE_ITEM_NOT_IN_USER_COMPLIANCE_SET = new WebLocalizationKeys("USER_COMPLIANCE_ITEM_NOT_IN_USER_COMPLIANCE_SET", "User Assistant Item is not in the User Assistant Set");
        public static readonly StringToken DELETE_ITEM = new WebLocalizationKeys("DELETE_ITEM", "Delete this item!");
        public static readonly StringToken EDIT_ITEM = new WebLocalizationKeys("EDIT_ITEM", "Edit this item!");
        public static readonly StringToken DISPLAY_ITEM = new WebLocalizationKeys("DISPLAY_ITEM", "Display this item!");
        public static readonly StringToken ASSISTANT = new WebLocalizationKeys("Assistant", "AssistantSet");
        public static readonly StringToken USER_HAS_DATA = new WebLocalizationKeys("USER_HAS_DATA", "User Has Data");
        public static readonly StringToken TASK_NAME = new WebLocalizationKeys("TASK_NAME", "Task Name");
        public static readonly StringToken NAME = new WebLocalizationKeys("NAME", "Name");
        public static readonly StringToken ADD_NEW_ITEM = new WebLocalizationKeys("ADD_NEW_ITEM", "Add New Item");
        public static readonly StringToken LICENSES = new WebLocalizationKeys("LICENSES", "Licenses");
        public static readonly StringToken LICENSE_TYPE = new WebLocalizationKeys("LICENSE_TYPE", "License Type");
        public static readonly StringToken PROFESSIONAL_CERTIFICATIONS = new WebLocalizationKeys("PROFESSIONAL_CERTIFICATIONS", "Professional Certifications");
        public static readonly StringToken PROFESSIONAL_CERTIFICATION_TYPE = new WebLocalizationKeys("PROFESSIONAL_CERTIFICATION_TYPE", "Professional Certification Type");
        public static readonly StringToken PROFESSIONAL_CERTIFICATION = new WebLocalizationKeys("PROFESSIONAL_CERTIFICATION", "Professional Certification");
        public static readonly StringToken TECHNICAL_CERTIFICATIONS = new WebLocalizationKeys("TECHNICAL_CERTIFICATIONS", "Technical Certifications");
        public static readonly StringToken TECHNICAL_CERTIFICATION_TYPE = new WebLocalizationKeys("TECHNICAL_CERTIFICATION_TYPE", "Technical Certification Type");
        public static readonly StringToken UPLOAD_NEW_DOCUMENT = new WebLocalizationKeys("UPLOAD_NEW_DOCUMENT", "Upload New Document");
        public static readonly StringToken SELECT_UPLOAD_FILE = new WebLocalizationKeys("SELECT_UPLOAD_FILE", "Select File to Upload");
        public static readonly StringToken DELETE_PHOTO = new WebLocalizationKeys("DELETE_PHOTO", "Delete Photo");

        public static readonly StringToken REMOVE_DOCUMENT = new WebLocalizationKeys("REMOVE_DOCUMENT", "(X) Remove Document");
        public static readonly StringToken DOCUMENTS = new WebLocalizationKeys("DOCUMENTS", "Documents");
        public static readonly StringToken FILES = new WebLocalizationKeys("FILES", "Files");
        public static readonly StringToken FILE = new WebLocalizationKeys("FILE", "File");
        public static readonly StringToken FILE_CATEGORY = new WebLocalizationKeys("FILE_CATEGORY", "File Category");
        public static readonly StringToken DOCUMENT = new WebLocalizationKeys("DOCUMENT", "Document");
        public static readonly StringToken PHOTO = new WebLocalizationKeys("PHOTO", "Photo");
        public static readonly StringToken PHOTOS = new WebLocalizationKeys("PHOTOS", "Photos");
        public static readonly StringToken SELECT_EXISTING_DOCUMENT = new WebLocalizationKeys("SELECT_EXISTING_DOCUMENT", "Select Existing Document");
        public static readonly StringToken UPLOAD_AND_SELECT = new WebLocalizationKeys("UPLOAD_AND_SELECT", "Upload and Select");
        public static readonly StringToken SELECT_ITEMS = new WebLocalizationKeys("SELECT_ITEMS", "Select checked items");
        public static readonly StringToken REMOVE_ITEMS = new WebLocalizationKeys("REMOVE_ITEMS", "Remove checked items");
        public static readonly StringToken REMOVE_ITEM = new WebLocalizationKeys("REMOVE_ITEM", "(X)Remove");
        public static readonly StringToken CATEGORY = new WebLocalizationKeys("CATEGORY", "Category");
        public static readonly StringToken FILETYPE = new WebLocalizationKeys("FILETYPE", "File Type");
        public static readonly StringToken SEARCH = new WebLocalizationKeys("SEARCH", "Search");
        public static readonly StringToken CLEAR = new WebLocalizationKeys("CLEAR", "Clear");
        public static readonly StringToken LICENSE_NUMBER = new WebLocalizationKeys("LICENSE_NUMBER", "License Number");
        public static readonly StringToken STATE = new WebLocalizationKeys("STATE", "State");
        public static readonly StringToken COMPLIANCE_SET = new WebLocalizationKeys("COMPLIANCE_SET", "Assistant Set");
        public static readonly StringToken CANCEL = new WebLocalizationKeys("CANCEL", "Cancel");
        public static readonly StringToken RETURN = new WebLocalizationKeys("RETURN", "Return");
        public static readonly StringToken ALL_ITEMS_HAVE_BEEN_COMPLETED = new WebLocalizationKeys("ALL_ITEMS_HAVE_BEEN_COMPLETED", "All of your items have been completed.  If you would like to create/update the portfolio for this set click the button.");
        public static readonly StringToken CREATE_PORTFOLIO = new WebLocalizationKeys("CREATE_PORTFOLIO", "Create Portfolio");
        public static readonly StringToken ADD = new WebLocalizationKeys("ADD", "Add");
        public static readonly StringToken COMPLIANCE_ITEM = new WebLocalizationKeys("COMPLIANCE_ITEM", "Assistant Item");

        public static readonly StringToken LAST_NAME = new WebLocalizationKeys("LAST_NAME", "Last Name");
        public static readonly StringToken FIRST_NAME = new WebLocalizationKeys("FIRST_NAME", "First Name");
        public static readonly StringToken DEPARTMENT = new WebLocalizationKeys("DEPARTMENT", "Department");
        public static readonly StringToken SHOW_MISSING = new WebLocalizationKeys("SHOW_MISSING", "Show Missing");
        public static readonly StringToken SHOW_ALL = new WebLocalizationKeys("SHOW_ALL", "Show All");
        public static readonly StringToken PORTFOLIO_STATUS_LIST = new WebLocalizationKeys("PORTFOLIO_STATUS_LIST", "Portfolio Status List");
        public static readonly StringToken UNIQUE_ID = new WebLocalizationKeys("UNIQUE_ID", "Unique Id");
        public static readonly StringToken PORTFOLIO_ITEMS = new WebLocalizationKeys("PORTFOLIO_ITEMS", "s");
        public static readonly StringToken SELECT_PORTFOLIO_ITEMS = new WebLocalizationKeys("SELECT_PORTFOLIO_ITEMS", "Select s");

        public static readonly StringToken DEGREES = new WebLocalizationKeys("DEGREES", "Degrees");
        public static readonly StringToken COUNTRY_IF_OBTAINED_OUTSIDE_US = new WebLocalizationKeys("COUNTRY_IF_OBTAINED_OUTSIDE_US", "Country (If obtained outside U.S.)");
        public static readonly StringToken DEGREE_LEVEL = new WebLocalizationKeys("DEGREE_LEVEL", "Degree Level");
        public static readonly StringToken DEGREE_IN_PROGRESS = new WebLocalizationKeys("DEGREE_IN_PROGRESS", "Degree In Progress");
        public static readonly StringToken DETAILS_IF_OTHER = new WebLocalizationKeys("DETAILS_IF_OTHER", "Details If Other");
        public static readonly StringToken DEGREE_AWARDED = new WebLocalizationKeys("DEGREE_AWARDED", "Degree Awarded");
        public static readonly StringToken MAJOR_S_ = new WebLocalizationKeys("MAJOR_S_", "Major(s)");
        public static readonly StringToken SCHOOL_NAME = new WebLocalizationKeys("SCHOOL_NAME", "School Name");
        public static readonly StringToken YEAR_AWARDED = new WebLocalizationKeys("YEAR_AWARDED", "Year Awarded");
        public static readonly StringToken GPA= new WebLocalizationKeys("GPA", "GPA");

        public static readonly StringToken CONTINUING_EDUCATION = new WebLocalizationKeys("CONTINUING_EDUCATION", "Continuing Education");
        public static readonly StringToken COURSE_OR_PRODUCT_ID = new WebLocalizationKeys("COURSE_OR_PRODUCT_ID", "Course/Product ID");
        public static readonly StringToken TYPE_OF_CONTINUING_EDUCATION = new WebLocalizationKeys("TYPE_OF_CONTINUING_EDUCATION", "Type Of Continuing Education");
        public static readonly StringToken TYPE_IF_OTHER = new WebLocalizationKeys("TYPE_IF_OTHER", "Type If Other");
        public static readonly StringToken RELEVANT_TO_ADVANCED_PRACTICE_NURSING = new WebLocalizationKeys("RELEVANT_TO_ADVANCED_PRACTICE_NURSING", "Relevant To Advanced Practice Nursing");

        public static readonly StringToken MEMBERSHIPS = new WebLocalizationKeys("MEMBERSHIPS", "Memberships");
        public static readonly StringToken MEMBERSHIP = new WebLocalizationKeys("MEMBERSHIP", "Membership");
        public static readonly StringToken OFFICES_AND_COMMITTEES = new WebLocalizationKeys("OFFICES_AND_COMMITTEES", "Offices And Committees");

        public static readonly StringToken COMMUNITY_SERVICE = new WebLocalizationKeys("COMMUNITY_SERVICE", "Community Service");
        public static readonly StringToken GROUP_OR_ORGANIZATION = new WebLocalizationKeys("GROUP_OR_ORGANIZATION", "Group Or Membership");
        public static readonly StringToken GROUP = new WebLocalizationKeys("GROUP", "Group");
        public static readonly StringToken ROLE_ON_PROJECT = new WebLocalizationKeys("ROLE_ON_PROJECT", "Role On Project");
        public static readonly StringToken TYPE_OF_SERVICE = new WebLocalizationKeys("TYPE_OF_SERVICE", "Type Of Service");
        public static readonly StringToken TO_PRESENT = new WebLocalizationKeys("TO_PRESENT", "To Present");

        public static readonly StringToken HONOR = new WebLocalizationKeys("HONOR", "Honor");
        public static readonly StringToken HONORS = new WebLocalizationKeys("HONORS", "Honors");
        public static readonly StringToken NAME_OF_HONOR = new WebLocalizationKeys("NAME_OF_HONOR", "Name Of Honor");
        public static readonly StringToken TYPE_OF_HONOR = new WebLocalizationKeys("TYPE_OF_HONOR", "Type Of Honor");
        public static readonly StringToken HONOR_GIVEN_BY = new WebLocalizationKeys("HONOR_GIVEN_BY", "Honor Given By");

        public static readonly StringToken TRAINING_SEMINAR = new WebLocalizationKeys("TRAINING_SEMINAR", "Training/Seminar");
        public static readonly StringToken TRAINING_SEMINARS = new WebLocalizationKeys("TRAINING_SEMINARS", "Training/Seminars");
        public static readonly StringToken TRAINING_SEMINAR_NAME = new WebLocalizationKeys("TRAINING_SEMINAR_NAME", "Training/Seminar Name");
        public static readonly StringToken FORUM_VENUE = new WebLocalizationKeys("FORUM_VENUE", "Forum/Venue");
        public static readonly StringToken ROLE_IF_OTHER = new WebLocalizationKeys("ROLE_IF_OTHER", "Role If Other");
        public static readonly StringToken FROM_DATE = new WebLocalizationKeys("FROM_DATE", "Start Date");
        public static readonly StringToken TO_DATE = new WebLocalizationKeys("TO_DATE", "End Date");
        public static readonly StringToken DISCIPLINE = new WebLocalizationKeys("DISCIPLINE ", "Discipline");
        public static readonly StringToken TRAINING_ROLE = new WebLocalizationKeys("TRAINING_ROLE ", "Training Role");
        public static readonly StringToken DISCIPLINE_OTHER = new WebLocalizationKeys("DISCIPLINE_OTHER", "Discipline If Other");
        
        public static readonly StringToken YEARS_OF_EXPERIENCE = new WebLocalizationKeys("YEARS_OF_EXPERIENCE", "Years of Experience");
        public static readonly StringToken LEVEL_OF_EXPERIENCE = new WebLocalizationKeys("LEVEL_OF_EXPERIENCE", "Level of Experience");
        public static readonly StringToken CLINICAL_EXPERIENCE = new WebLocalizationKeys("CLINICAL_EXPERIENCE", "Clinical Experience");
        public static readonly StringToken EXPERIENCE_CURRENT = new WebLocalizationKeys("EXPERIENCE_CURRENT", "Is Experience Current?");
        public static readonly StringToken LENGTH = new WebLocalizationKeys("LENGTH", "Length");

        public static readonly StringToken PRESENTATION_FORUM = new WebLocalizationKeys("PRESENTATION_FORUM", "Presentation Forum");
        public static readonly StringToken PRESENTATION = new WebLocalizationKeys("PRESENTATION", "Presentation");
        public static readonly StringToken PRESENTATIONS = new WebLocalizationKeys("PRESENTATIONS", "Presentations");
        public static readonly StringToken PRESENTATION_TITLE = new WebLocalizationKeys("PRESENTATION_TITLE", "Presentation Title");
        public static readonly StringToken PRESENTATION_TYPE = new WebLocalizationKeys("PRESENTATION_TYPE", "Presentation Type");
        public static readonly StringToken LINK_TO_URL = new WebLocalizationKeys("LINK_TO_URL", "Link To URL");

        public static readonly StringToken COMMITTEE = new WebLocalizationKeys("COMMITTEE", "Committee");
        public static readonly StringToken COMMITTEES = new WebLocalizationKeys("COMMITTEES", "Committees");
        public static readonly StringToken COMMITTEE_NAME = new WebLocalizationKeys("COMMITTEE_NAME", "Committee Name");

        public static readonly StringToken EMPLOYER_GROUP = new WebLocalizationKeys("EMPLOYER_GROUP", "Employer/Group");
        public static readonly StringToken EMPLOYER_NAME_AND_ADDRESS = new WebLocalizationKeys("EMPLOYER_NAME_AND_ADDRESS", "Employer Name and Address");
        public static readonly StringToken HOURS_PER_WEEK= new WebLocalizationKeys("HOURS_PER_WEEK", "Hours Per Week");
        public static readonly StringToken ROLE_ON_COMMITTEE = new WebLocalizationKeys("ROLE_ON_COMMITTEE", "Role On Committee");

        public static readonly StringToken PUBLICATION_IS_IN_PRESS = new WebLocalizationKeys("PUBLICATION_IS_IN_PRESS", "In Press");

        public static readonly StringToken PUBLICATION = new WebLocalizationKeys("PUBLICATION", "Publication");
        public static readonly StringToken PUBLICATIONS = new WebLocalizationKeys("PUBLICATIONS", "Publications");
        public static readonly StringToken PUBLICATION_TITLE = new WebLocalizationKeys("PUBLICATION_TITLE", "Publication Title");
        public static readonly StringToken ROLE = new WebLocalizationKeys("ROLE", "Role");
        public static readonly StringToken ROLE_DESCRIPTION = new WebLocalizationKeys("ROLE_DESCRIPTION", "Role/Description");
        public static readonly StringToken VOLUME_ISSUE = new WebLocalizationKeys("VOLUME_ISSUE", "Volume/Issue");
        public static readonly StringToken ORGANIZATION = new WebLocalizationKeys("ORGANIZATION", "Organization");

        public static readonly StringToken REQUIRED_BY_SYSTEM = new WebLocalizationKeys("REQUIRED_BY_SYSTEM", "Is Required By the System");
        public static readonly StringToken THIS_COMPLIANCE_ITEM_COMPLETE = new WebLocalizationKeys("THIS_COMPLIANCE_ITEM_COMPLETE", "The {0} Assistant Item is Complete");
        public static readonly StringToken REQUIRED_NUMBER_OF_ITEMS_HAVE_NOT_BEEN_ENTERED = new WebLocalizationKeys("REQUIRED_NUMBER_OF_ITEMS_HAVE_NOT_BEEN_ENTERED", "The Required Number of {0} Comliance Items Have Not Been Entered");
        public static readonly StringToken THIS_COMPLIANCE_ITEM_IS_MISSING_REQUIRED_FIELDS = new WebLocalizationKeys("THIS_COMPLIANCE_ITEM_IS_MISSING_REQUIRED_FIELDS", "The {0} Assistant Item is Missing Required Fields");

        public static readonly StringToken YOU_HAVE_NOT_ADDED_ANY = new WebLocalizationKeys("YOU_HAVE_NOT_ADDED_ANY", "You have not added any {0} ");
        public static readonly StringToken SELECT_ONE_OR_MORE_OR = new WebLocalizationKeys("SELECT_ONE_OR_MORE", "Select one or more or ...");
        public static readonly StringToken ADD_A_NEW = new WebLocalizationKeys("ADD_A_NEW", "Add a new {0}");
        public static readonly StringToken ADD_A_NEW_ONE = new WebLocalizationKeys("ADD_A_NEW_ONE", "add a new one");
        public static readonly StringToken CHECK_HERE_IF_YOU_ARE_DONE_OR_THIS_IS_NOT_APPLICABLE = new WebLocalizationKeys("CHECK_HERE_IF_YOU_ARE_DONE_OR_THIS_IS_NOT_APPLICABLE", "Check here if you are done with this section or if this section does not apply to you.");

        public static readonly StringToken GET_STARTED = new WebLocalizationKeys("GET_STARTED", "Get Started");
        public static readonly StringToken ADD_OR_EDIT = new WebLocalizationKeys("ADD_OR_EDIT", "Add or Edit");
        public static readonly StringToken COMPLETE = new WebLocalizationKeys("COMPLETE", "Complete");
        public static readonly StringToken INFO_SAVED_BUT_NOT_COMPLETE = new WebLocalizationKeys("INFO_SAVED_BUT_NOT_COMPLETE", "Your data was saved but this item can not be concidered complete for the following reasons:");
        public static readonly StringToken PREVIEW = new WebLocalizationKeys("PREVIEW", "Preview");
        public static readonly StringToken SHARE = new WebLocalizationKeys("SHARE", "Share");
        public static readonly StringToken LAST_NAME_FIRST_NAME = new WebLocalizationKeys("LAST_NAME_FIRST_NAME", "Last Name, FirsteName");
        public static readonly StringToken PERCENT_COMPLETE = new WebLocalizationKeys("PERCENT_COMPLETE", "% Complete");
        public static readonly StringToken MANAGER = new WebLocalizationKeys("MANAGER", "Manager");
        public static readonly StringToken ASSISTANT_PERCENT_COMPLETE_REPORT = new WebLocalizationKeys("ASSISTANT_PERCENT_COMPLETE_REPORT", "Assistant % Complete Report");
        public static readonly StringToken DISPLAYING_THOSE_WHO_HAVE_NOT_COMPLETED_ASSISTANT = new WebLocalizationKeys("DISPLAYING_THOSE_WHO_HAVE_NOT_COMPLETED_ASSISTANT", "Displaying the {0} recipients who have not completed the assistant");
        public static readonly StringToken GROUP_BY = new WebLocalizationKeys("GROUP_BY", "Group By");

        public static readonly StringToken NAME_OF_FUNDED_ACTIVITY = new WebLocalizationKeys("NAME_OF_FUNDED_ACTIVITY", "Name Of Funded Activity");
        public static readonly StringToken AMOUNT_RECEIVED_FOR_FUNDED_ACTIVITY = new WebLocalizationKeys("AMOUNT_RECEIVED_FOR_FUNDED_ACTIVITY", "Amount Received For Activity");
        public static readonly StringToken CONTRACT_ITEM_HEADER = new WebLocalizationKeys("CONTRACT_ITEM_HEADER", "Contract Name");

        public static readonly StringToken MILITARY_SERVICE = new WebLocalizationKeys("MILITARY_SERVICE", "Military Service");
        public static readonly StringToken SERVICE_TYPE = new WebLocalizationKeys("SERVICE_TYPE", "Service Type");
        public static readonly StringToken SERVICE_BRANCH = new WebLocalizationKeys("SERVICE_BRANCH", "Service Branch");
        public static readonly StringToken SERVICE_ENTRY_DATE = new WebLocalizationKeys("SERVICE_ENTRY_DATE", "Service Entry Date");
        public static readonly StringToken SERVICE_DISCHARGE_DATE = new WebLocalizationKeys("SERVICE_DISCHARGE_DATE", "Discharge Date");
        public static readonly StringToken SERVICE_TYPE_OF_DISCHARGE = new WebLocalizationKeys("SERVICE_TYPE_OF_DISCHARGE", "Type of Discharge");
        public static readonly StringToken SERVICE_HIGHEST_RANK_ACHIEVED = new WebLocalizationKeys("SERVICE_HIGHEST_RANK_ACHIEVED", "Highest Rank Achieved");
        
        public static readonly StringToken ACADEMIC_COURSES = new WebLocalizationKeys("ACADEMIC_COURSES", "Academic Courses");
        public static readonly StringToken ACADEMIC_COURSE = new WebLocalizationKeys("ACADEMIC_COURSE", "Academic Course");
        
        public static readonly StringToken TEACHING_EXPERIENCE = new WebLocalizationKeys("TEACHING_EXPERIENCE", "Teaching Experience");
        public static readonly StringToken CONSULTING_ACTIVITIES = new WebLocalizationKeys("CONSULTING_ACTIVITIES", "Consulting Activities");
        public static readonly StringToken CONSULTING_ACTIVITY = new WebLocalizationKeys("CONSULTING_ACTIVITY", "Consulting Activity");
        public static readonly StringToken ACTIVITY_NAME = new WebLocalizationKeys("ACTIVITY_NAME", "Activity Name");
        public static readonly StringToken FUNDED_ACTIVITIES = new WebLocalizationKeys("FUNDED_ACTIVITIES", "Funded Activities");
        public static readonly StringToken FUNDED_ACTIVITY = new WebLocalizationKeys("FUNDED_ACTIVITY", "Funded Activity");
        public static readonly StringToken CONTRACTS = new WebLocalizationKeys("CONTRACTS", "Contracts");
        public static readonly StringToken CONTRACT = new WebLocalizationKeys("CONTRACT", "Contract");
        public static readonly StringToken ADDRESSES = new WebLocalizationKeys("ADDRESSES", "Addresses");
        public static readonly StringToken PHONE_NUMBERS = new WebLocalizationKeys("PHONE_NUMBERS", "Phone Numbers");
        public static readonly StringToken EMAIL_ADDRESSES = new WebLocalizationKeys("EMAIL_ADDRESSES", "Email Addresses");
        public static readonly StringToken ASSOCIATED_DOCUMENTS = new WebLocalizationKeys("ASSOCIATED_DOCUMENTS", "Associated Documents");
        public static readonly StringToken RESEARCH = new WebLocalizationKeys("RESEARCH", "Research");
        public static readonly StringToken THESIS = new WebLocalizationKeys("THESIS", "Thesis");
        public static readonly StringToken RESEARCH_TITLE = new WebLocalizationKeys("RESEARCH_TITLE", "Research Title");
        public static readonly StringToken RESEARCH_PROJECT = new WebLocalizationKeys("RESEARCH_PROJECT", "Research Project");
      
        public static readonly StringToken IRB_APPROVAL = new WebLocalizationKeys("IRB_APPROVAL", "I.R.B. Approval");
        public static readonly StringToken RESEARCH_FOR = new WebLocalizationKeys("RESEARCH_FOR", "Research For");

        public static readonly StringToken THESIS_ROLETYPE = new WebLocalizationKeys("THESIS_ROLETYPE", "Thesis Role Type");
        public static readonly StringToken THESIS_COMMITTEE_MEMBERS = new WebLocalizationKeys("THESIS_COMMITTEE_MEMBERS", "Thesis Committee Members");
        public static readonly StringToken THESIS_DATE_DEFENDED = new WebLocalizationKeys("THESIS_DATE_DEFENDED", "Thesis Date Defended");

        public static readonly StringToken FELLOWSHIP_TITLE = new WebLocalizationKeys("FELLOWSHIP_TITLE", "Fellowship");
        public static readonly StringToken FELLOWSHIPS_TITLE = new WebLocalizationKeys("FELLOWSHIPS_TITLE", "Fellowships");
        public static readonly StringToken FELLOWSHIP_NAME = new WebLocalizationKeys("FELLOWSHIP_NAME", "Fellowship Name");
        public static readonly StringToken FELLOWSHIP_DATE = new WebLocalizationKeys("FELLOWSHIP_DATE", "Fellowship Date");
        public static readonly StringToken FELLOWSHIPS = new WebLocalizationKeys("FELLOWSHIPS", "Fellowships");
        public static readonly StringToken FELLOWSHIP_PROVIDER = new WebLocalizationKeys("FELLOWSHIP_PROVIDER", "Fellowship Provider");


        public static readonly StringToken SOFTWARE = new WebLocalizationKeys("SOFTWARE_TITLE", "Software");
        public static readonly StringToken RESET_PASSWORD = new WebLocalizationKeys("RESET_PASSWORD", "Reset Password");
        public static readonly StringToken NEW_PASSWORD = new WebLocalizationKeys("NEW_PASSWORD", "New Password");
        public static readonly StringToken CONFIRM_NEW_PASSWORD = new WebLocalizationKeys("CONFIRM_NEW_PASSWORD", "Confirm New Password");
        public static readonly StringToken LOGIN_NAME = new WebLocalizationKeys("LOGIN_NAME", "Login Name");
        public static readonly StringToken PLEASE_SELECT_A_PASSWORD = new WebLocalizationKeys("PLEASE_SELECT_A_PASSWORD", "Hello {0}, please select a new password");
        public static readonly StringToken DEVELOPMENT_DATE = new WebLocalizationKeys("DEVELOPMENT_DATE", "Development Date");

        public static readonly StringToken WORK_HISTORY = new WebLocalizationKeys("WORK_HISTORY", "Work History");
        public static readonly StringToken PAY_FREQUENCY = new WebLocalizationKeys("PAY_FREQUENCY", "Pay Frequency");
        public static readonly StringToken SUPERVISOR = new WebLocalizationKeys("SUPERVISOR", "Supervisor");
        public static readonly StringToken SUPERVISOR_PHONE = new WebLocalizationKeys("SUPERVISOR_PHONE", "Supervisor Phone");
        public static readonly StringToken SUPERVISOR_CAN_CONTACT = new WebLocalizationKeys("SUPERVISOR_CAN_CONTACT", "Can Contact Supervisor");

        public static readonly StringToken DECISION_CRITICAL_EMAIL_CONFIRMATION = new WebLocalizationKeys("DECISION_CRITICAL_EMAIL_CONFIRMATION", "Decision Critical Email Confirmation");
        public static readonly StringToken ERROR_IN_EMAIL_CONFIRMATION = new WebLocalizationKeys("ERROR_IN_EMAIL_CONFIRMATION", "Were sorry there was an error confirming your email address. Please try again");
        public static readonly StringToken YOUR_EMAIL_ADDRESS_HAS_BEEN_CONFIRMED = new WebLocalizationKeys("YOUR_EMAIL_ADDRESS_HAS_BEEN_CONFIRMED", "Your email address has been confirmed.  Thank you.");
        public static readonly StringToken STUDENT_ACTIVITIES_TITLE = new WebLocalizationKeys("STUDENT_ACTIVITIES_TITLE", "Student Activities");
        public static readonly StringToken ACCOMPLISHMENTS = new WebLocalizationKeys("ACCOMPLISHMENTS", "Accomplishments");
        public static readonly StringToken STUDENT_ACTIVITY_TITLE = new WebLocalizationKeys("STUDENT_ACTIVITY_TITLE", "Student Activity");
        public static readonly StringToken REFLECTION_TITLE = new WebLocalizationKeys("REFLECTION_TITLE", "Reflection");
        public static readonly StringToken COMPLIANCE_ITEM_TO_EXPIRE_SOON = new WebLocalizationKeys("COMPLIANCE_ITEM_TO_EXPIRE_SOON", "Compliance Item to Expire Soon");
        public static readonly StringToken GOAL = new WebLocalizationKeys("GOAL", "Goal");
        public static readonly StringToken GOALS = new WebLocalizationKeys("GOALS", "Goals");
        public static readonly StringToken GOAL_MET = new WebLocalizationKeys("GOAL_MET", "Goal Met");
        public static readonly StringToken SEND_EMAIL_REMINDERS = new WebLocalizationKeys("SEND_EMAIL_REMINDERS", "Send Reminder Emails?");
        public static readonly StringToken SHOW_ON_DASHBOARD = new WebLocalizationKeys("SHOW_ON_DASHBOARD", "Show on Dashbaord?");
        public static readonly StringToken COMPLIANCE_ITEM_EXPIRES_TODAY = new WebLocalizationKeys("COMPLIANCE_ITEM_EXPIRES_TODAY", "Compliance Item to Expires Today");
        public static readonly StringToken COMPLIANCE_ITEM_EXPIRED = new WebLocalizationKeys("COMPLIANCE_ITEM_EXPIRED", "Compliance Item Has Expired!");
        public static readonly StringToken SELECT_THE_NOTIFICATION_SCHEDULE_YOU_WOULD_LIKE = new WebLocalizationKeys("SELECT_THE_NOTIFICATION_SCHEDULE_YOU_WOULD_LIKE", "Select the notification schedule you would like");
        public static readonly StringToken DAYS_BEFORE_ITEM_EXPIRES = new WebLocalizationKeys("DAYS_BEFORE_ITEM_EXPIRES", "Days before item expires (0 to 60)");
        public static readonly StringToken FREQUENCEY_TO_SEND_NOTIFICATION = new WebLocalizationKeys("FREQUENCEY_TO_SEND_NOTIFICATION", "Frequency to send notificaions (0 to 60)");
        public static readonly StringToken SEND_NOTIFICATION_THE_DAY_OF_EXPIRATION = new WebLocalizationKeys("SEND_NOTIFICATION_THE_DAY_OF_EXPIRATION", "Send notification the day of expiration?");
        public static readonly StringToken AFTER_EXPIRATION_FREQUENCY_TO_SEND_NOTIFICATION = new WebLocalizationKeys("AFTER_EXPIRATION_FREQUENCY_TO_SEND_NOTIFICATION", "After expiration, frequency to send notification (0 to 60)");
        public static readonly StringToken STOP_SENDING_NOTIFICATIONS_AFTER_DAYS = new WebLocalizationKeys("STOP_SENDING_NOTIFICATIONS_AFTER_DAYS", "Number of day after which to stop sending notifications (0 to 60)");
        public static readonly StringToken COMPLIANCE_SETTINGS = new WebLocalizationKeys("COMPLIANCE_SETTINGS", "Compliance Settings");
        public static readonly StringToken COMPLIANCE_ITEMS = new WebLocalizationKeys("COMPLIANCE_ITEMS", "Compliance Items");
        public static readonly StringToken INTERVIEWS = new WebLocalizationKeys("INTERVIEWS", "Interviews");
        public static readonly StringToken INTERVIEW = new WebLocalizationKeys("INTERVIEW", "Interview");
        public static readonly StringToken REVIEWS = new WebLocalizationKeys("REVIEWS", "Reviews");
        public static readonly StringToken GRANT = new WebLocalizationKeys("GRANT", "Grant");
        public static readonly StringToken GRANTS = new WebLocalizationKeys("GRANTS", "Grants");
        public static readonly StringToken GRANT_ROLE = new WebLocalizationKeys("GRANT_ROLE", "Role");
        public static readonly StringToken GRANT_NAME = new WebLocalizationKeys("GRANT_NAME", "Grant Name");
        public static readonly StringToken GRANT_STATUS = new WebLocalizationKeys("GRANT_STATUS", "Grant Status");
        public static readonly StringToken DATE_AWARDED = new WebLocalizationKeys("DATE_AWARDED", "Date Awarded");
        public static readonly StringToken DATE_COMPLETED = new WebLocalizationKeys("DATE_COMPLETED", "Date Completed");
        

        
        public static readonly StringToken HEALTH_POLICIES = new WebLocalizationKeys("HEALTH_POLICIES", "Health Policies");
        public static readonly StringToken HEALTH_POLICY = new WebLocalizationKeys("HEALTH_POLICY", "Health Policy");

        public static readonly StringToken NO_DOCUMENT_UPLOADED = new WebLocalizationKeys("NO_DOCUMENT_UPLOADED", "No Document Uploaded");
        public static readonly StringToken ASSOCIATED_PHOTOS = new WebLocalizationKeys("ASSOCIATED_PHOTOS", "Associated Photos");
        public static readonly StringToken PORTFOLIO_NAME = new WebLocalizationKeys("PORTFOLIO_NAME", "Portflio Name");

        public static readonly StringToken PHOTO_TITLES = new WebLocalizationKeys("PHOTO_TITLES", "Photos");
        public static readonly StringToken ASSETS_TITLE = new WebLocalizationKeys("ASSETS_TITLE", "Assets");
        public static readonly StringToken SAVE = new WebLocalizationKeys("SAVE", "Save");
        public static readonly StringToken FILE_URL = new WebLocalizationKeys("FILE_URL", "File URL");
        public static readonly StringToken CLICK_TO_VIEW_FILE = new WebLocalizationKeys("CLICK_TO_VIEW_FILE", "View File");

        public static readonly StringToken CREATED_DATE = new WebLocalizationKeys("CREATED_DATE", "Created Date");
        public static readonly StringToken LAST_EDITED = new WebLocalizationKeys("LAST_EDITED", "Last Edited");

        public static readonly StringToken REPOSITORY = new WebLocalizationKeys("REPOSITORY", "Repository");
        public static readonly StringToken HISTORY = new WebLocalizationKeys("HISTORY", "History");
        public static readonly StringToken PROFESSIONAL_EXPERIENCE = new WebLocalizationKeys("PROFESSIONAL_EXPERIENCE", "Professional Experience");
        
        
        
        public static readonly StringToken SECTION1 = new WebLocalizationKeys("SECTION1", "Employment");
        public static readonly StringToken SECTION2 = new WebLocalizationKeys("SECTION2", "Education");
        public static readonly StringToken SECTION3 = new WebLocalizationKeys("SECTION3", "Credentials");
        public static readonly StringToken SECTION4 = new WebLocalizationKeys("SECTION4", "Professional Experience");
        public static readonly StringToken SECTION5 = new WebLocalizationKeys("SECTION5", "Scholarly Activities");
        public static readonly StringToken SECTION6 = new WebLocalizationKeys("SECTION6", "Other");

        public static readonly StringToken COMPLIANCE_ITEM_REQUIRED = new WebLocalizationKeys("COMPLIANCE_ITEM_REQUIRED", "You must select at least one Compliance Item");
        public static readonly StringToken EMAIL_ADDRESSES_REQUIRED = new WebLocalizationKeys("EMAIL_ADDRESSES_REQUIRED", "You must select at least one Email Address");
        public static readonly StringToken DATE_ADDED = new WebLocalizationKeys("DATE_ADDED", "Added {0}");
        public static readonly StringToken NEW = new WebLocalizationKeys("NEW", "New ");
        public static readonly StringToken ADD_NEW = new WebLocalizationKeys("ADD_NEW", "Add New ");
        public static readonly StringToken TITLE = new WebLocalizationKeys("TITLE", "Title");
        public static readonly StringToken HELD_FROM = new WebLocalizationKeys("HELD_FROM", "Held From");
        public static readonly StringToken HELD_TO = new WebLocalizationKeys("HELD_TO", "Held To");
        public static readonly StringToken INIT = new WebLocalizationKeys("INIT", "Init.");
        public static readonly StringToken CREDENTIALS = new WebLocalizationKeys("CREDENTIALS", "Credentials");
        public static readonly StringToken POSITIONS = new WebLocalizationKeys("POSITIONS", "Positions");
        public static readonly StringToken AUTHORS = new WebLocalizationKeys("AUTHORS", "Authors");

        public static readonly StringToken PERSONAL_INFORMATION = new WebLocalizationKeys("PERSONAL_INFORMATION", "Personal Information");
        public static readonly StringToken PASSWORD = new WebLocalizationKeys("PASSWORD", "Password");
        public static readonly StringToken INITIAL = new WebLocalizationKeys("INITIAL", "Initial");
        public static readonly StringToken MY_ACCOUNT_INFORMATION = new WebLocalizationKeys("MY_ACCOUNT_INFORMATION", "My Account Information");
        public static readonly StringToken IS_PRESENT_EMPLOYED = new WebLocalizationKeys("IS_PRESENT_EMPLOYED", "Is Presently Employed");
        public static readonly StringToken BUILD_NEW_PORTFOLIO = new WebLocalizationKeys("BUILD_NEW_PORTFOLIO", "Build New Portfolio");
        public static readonly StringToken UPDATE_EXISTING_PORTFOLIO= new WebLocalizationKeys("UPDATE_EXISTING_PORTFOLIO", "Update Existing Portfolio");
        public static readonly StringToken ADD_FILES= new WebLocalizationKeys("ADD_FILES", "Add Files");
        public static readonly StringToken ADD_COVER_TEXT= new WebLocalizationKeys("ADD_COVER_TEXT", "Add Cover Text");
        public static readonly StringToken ADD_REFLECTIONS = new WebLocalizationKeys("ADD_REFLECTIONS", "Add Reflections");
        public static readonly StringToken ADD_EXPERIENCE_ITEMS = new WebLocalizationKeys("ADD_EXPERIENCE_ITEMS", "Add Professional Experience Items");
        public static readonly StringToken CONTACT_INFO = new WebLocalizationKeys("CONTACT_INFO", "Contact Info");
        public static readonly StringToken ADD_NEW_HEADSHOT = new WebLocalizationKeys("ADD_NEW_HEADSHOT", "Add New Head Shot");
        public static readonly StringToken COVER_TEXTS = new WebLocalizationKeys("COVER_TEXTS", "Cover Texts");
        public static readonly StringToken COVER_TEXT = new WebLocalizationKeys("COVER_TEXT", "Cover Text");
        public static readonly StringToken COVER_TEXT_NAME = new WebLocalizationKeys("COVER_TEXT_NAME", "Cover Text Name");
        public static readonly StringToken PORTFOLIO = new WebLocalizationKeys("PORTFOLIO", "Portfolio");
        public static readonly StringToken EMAIL_PORTFOLIO_SUBJECT_LINE = new WebLocalizationKeys("EMAIL_PORTFOLIO_SUBJECT_LINE", "Please View My Portfolio");
        public static readonly StringToken ISSUED = new WebLocalizationKeys("ISSUED", "Issued");
        public static readonly StringToken EXPIRES = new WebLocalizationKeys("EXPIRES", "Expires");
        public static readonly StringToken JOB_TITLE = new WebLocalizationKeys("JOB_TITLE", "Job Title");
        public static readonly StringToken CONTACT_HOURS = new WebLocalizationKeys("CONTACT_HOURS", "Contact Hours");
        public static readonly StringToken CREDIT_HOURS = new WebLocalizationKeys("CREDIT_HOURS", "Credit Hours");
        public static readonly StringToken HEADSHOT = new WebLocalizationKeys("HEADSHOT", "Headshot");
        public static readonly StringToken HEADSHOT_NAME = new WebLocalizationKeys("HEADSHOT_NAME", "Headshot Name");
        public static readonly StringToken HEADSHOTS = new WebLocalizationKeys("HEADSHOTS", "Headshots");
        public static readonly StringToken PORTFOLIO_PREVIEW = new WebLocalizationKeys("PORTFOLIO_PREVIEW", "Portfolio Preview");
        public static readonly StringToken ADD_PHOTO = new WebLocalizationKeys("ADD_PHOTO", "Add Photo");

        //////////////////////////
        public static readonly StringToken CALNEDAR = new WebLocalizationKeys("CALNEDAR", "Calendar");
        public static readonly StringToken LEARNING = new WebLocalizationKeys("LEARNING", "Learning");
        public static readonly StringToken EVALUATIONS = new WebLocalizationKeys("EVALUATIONS", "Evaluations");
        //////////////////////////

        public static readonly StringToken COPY_ITEMS = new WebLocalizationKeys("COPY_ITEMS", "Copy Selected Items");
        public static readonly StringToken ADDTO_PORT_BUTTON_TITLE = new WebLocalizationKeys("ADDTO_PORT_BUTTON_TITLE", "Add to Portfolio");
        public static readonly StringToken REMOVE = new WebLocalizationKeys("REMOVE", "Remove");
        public static readonly StringToken DOWNLOAD = new WebLocalizationKeys("DOWNLOAD", "Download");
        public static readonly StringToken PRINT = new WebLocalizationKeys("PRINT", "Print");
        public static readonly StringToken CLOSE = new WebLocalizationKeys("CLOSE", "Close");
        public static readonly StringToken EMAIL = new WebLocalizationKeys("EMAIL", "Email");
        public static readonly StringToken ERROR_HEADER = new WebLocalizationKeys("ERROR_HEADER", "Oops");
        public static readonly StringToken ERROR_BODY = new WebLocalizationKeys("ERROR_BODY", "Sorry, an error occurred while processing your request.");
        public static readonly StringToken ERROR_UNEXPECTED = new WebLocalizationKeys("ERROR_UNEXPECTED", "Sorry, an unexpected error occurred while processing your request.");



        public static readonly StringToken REFLECTIONS = new WebLocalizationKeys("REFLECTIONS", "Reflections");
        public static readonly StringToken NO_EXISTING_PORTFOLIOS = new WebLocalizationKeys("NO_EXISTING_PORTFOLIOS", "You have no existing Portfolios");
        public static readonly StringToken ADD_A_PORTFOLIO = new WebLocalizationKeys("ADD_A_PORTFOLIO", "Add a Portfolios");
        public static readonly StringToken CHOOSE_PORTFOLIO_TO_ADD_ITEMS_TO = new WebLocalizationKeys("CHOOSE_PORTFOLIO_TO_ADD_ITEMS_TO", "Choose a Portfolio to add items too");
        public static readonly StringToken ADD_TO_A_PORTFOLIO = new WebLocalizationKeys("ADD_TO_A_PORTFOLIO", "Add to a Portfolios");

        public static readonly StringToken FIELD_OF_STUDY = new WebLocalizationKeys("FIELD_OF_STUDY", "Field of Study");
        public static readonly StringToken ADVANCED_PRACTICE_TYPE = new WebLocalizationKeys("ADVANCED_PRACTICE_TYPE", "Advance Practice Type");

        public static readonly StringToken HONOR_TYPE = new WebLocalizationKeys("HONOR_TYPE", "Honor Type");
        public static readonly StringToken PUBLICATION_TYPE = new WebLocalizationKeys("PUBLICATION_TYPE", "Publication Type");
        public static readonly StringToken NEW_PORTFOLIO_NAME = new WebLocalizationKeys("NEW_PORTFOLIO_NAME", "New Portfolio");


        public static readonly StringToken SEND_PORTFOLIO = new WebLocalizationKeys("SEND_PORTFOLIO", "Send To: (separate multiple email addresses with a semi-colon)");
        public static readonly StringToken SUBJECT = new WebLocalizationKeys("SUBJECT", "Subject");
        public static readonly StringToken FROM = new WebLocalizationKeys("FROM", "From");
        public static readonly StringToken SEND_TO = new WebLocalizationKeys("SEND_TO", "Send To");
        public static readonly StringToken ALL_EXPERIENCE = new WebLocalizationKeys("ALL_EXPERIENCE", "All Experience");
        public static readonly StringToken ADD_THIS_ITEM = new WebLocalizationKeys("ADD_THIS_ITEM", "Add This Item");
        public static readonly StringToken AGENCY_JOURNAL = new WebLocalizationKeys("AGENCY_JOURNAL", "Agency, Journal, Title");
        public static readonly StringToken NAME_OF_SOFTWARE = new WebLocalizationKeys("NAME_OF_SOFTWARE", "Name of Software");
        public static readonly StringToken ADD_HOVER = new WebLocalizationKeys("ADD_HOVER", "Add");
        public static readonly StringToken OPTIONAL_PROMOCODE = new WebLocalizationKeys("OPTIONAL_PROMOCODE", "Promotion Code (optional)");
        public static readonly StringToken CONTINUE = new WebLocalizationKeys("CONTINUE", "Continue");
        public static readonly StringToken PROMO_INVALID = new WebLocalizationKeys("PROMO_INVALID", "The Promotion Code you entered is not valid");

    }
}