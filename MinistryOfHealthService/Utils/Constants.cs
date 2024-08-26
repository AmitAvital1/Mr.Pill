namespace MrPill.MOHService.Constants;

public class Constants
{
    public static readonly string MOH_API_URL = "https://israeldrugs.health.gov.il/GovServiceList/IDRServer/SearchByAdv";
    public static readonly string MOH_JSON_BARCODES_KEY = "barcodes";
    public static readonly string MOH_JSON_DRAG_HEB_NAME_KEY = "dragHebName";
    public static readonly string MOH_JSON_DRAG_ENG_NAME_KEY = "dragEnName";
    public static readonly string MOH_JSON_DRAG_ENG_DESC_KEY = "indications";
    public static readonly string MOH_JSON_DRAG_HEB_DESC_KEY = "secondarySymptom";
    public static readonly string MOH_JSON_DRAG_IMG_PATH_KEY = "images";
    public static readonly string MOH_JSON_PACKAGES_KEY = "packages";
    public static readonly string MOH_JSON_REG_DATE_KEY = "dragRegDate";
    public static readonly string MOH_JSON_BITUL_DATE_KEY = "bitulDate";
    public static readonly string MOH_IMAGE_BASE_URL = "https://mohpublic.z6.web.core.windows.net/IsraelDrugs/";

}

// dragRegDate :

    // What it Represents: 
        // This field represents the registration date of the medication.
        // It indicates the date on which the medication was officially registered or approved for use by the relevant health authority.
    // Purpose: 
        // The registration date is important for understanding when the medication became available on the market 
        // and is often used to track the validity of the medication's approval.


// bitulDate :

    // What it Represents: 
        // This field represents the cancellation date or invalidation date of the medication. 
        // It indicates when the medication was withdrawn, canceled, or is no longer considered valid for use.
    // Special Case: 
        // A date like 01/01/1900 is often used as a placeholder or default value to indicate that the medication has not 
        // been canceled or invalidated and is still valid. 
        // This date is not intended to be taken literally but rather as a signal that there is no cancellation.
    // Purpose: 
        // The cancellation date is important for determining whether a medication is still approved for use. 
        // If this date is set to something other than the default, it usually means the medication has been taken off the market.