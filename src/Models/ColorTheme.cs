namespace ColorThemes.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a comprehensive set of theme colors for your app.
/// 
/// These colors can be used with AppThemeResource in XAML:
/// <code>
/// &lt;Button Background="{toolkit:AppThemeResource Primary}" /&gt;
/// </code>
/// 
/// Make sure to include the toolkit namespace in your XAML:
/// <code>
/// xmlns:toolkit="http://schemas.microsoft.com/dotnet/2022/maui/toolkit"
/// </code>
/// </summary>
public class ColorTheme
{
    // Default constructor that initializes all properties
    public ColorTheme()
    {
        // Initialize with default colors
        Title = "New Theme";
        
        // Initialize default hex colors
        PrimaryHex = "#0078D4";
        PrimaryDarkHex = "#2B88D8";
        SecondaryHex = "#6E6E6E";
        SecondaryDarkHex = "#9E9E9E";
        BackgroundHex = "#FFFFFF";
        BackgroundDarkHex = "#121212";
        Surface0Hex = "#FFFFFF";
        Surface0DarkHex = "#121212";
        Surface1Hex = "#F5F5F5";
        Surface1DarkHex = "#1E1E1E";
        Surface2Hex = "#EEEEEE";
        Surface2DarkHex = "#222222";
        Surface3Hex = "#E0E0E0";
        Surface3DarkHex = "#242424";
        OnBackgroundHex = "#212121";
        OnBackgroundDarkHex = "#F5F5F5";
        OnSurfaceHex = "#212121";
        OnSurfaceDarkHex = "#F5F5F5";
        OnPrimaryHex = "#FFFFFF";
        OnPrimaryDarkHex = "#121212";
        OnSecondaryHex = "#FFFFFF";
        OnSecondaryDarkHex = "#121212";
        ErrorHex = "#B71C1C";
        ErrorDarkHex = "#EF5350";
        SuccessHex = "#1B5E20";
        SuccessDarkHex = "#66BB6A";
        InfoHex = "#0D47A1";
        InfoDarkHex = "#42A5F5";
        
        // Now update colors from hex strings
        UpdateColorsFromHex();
    }
    
    /// <summary>
    /// Updates all Color objects based on their hex string counterparts
    /// </summary>
    public void UpdateColorsFromHex()
    {
        // Light theme colors
        Primary = Color.FromArgb(PrimaryHex);
        Secondary = Color.FromArgb(SecondaryHex);
        Background = Color.FromArgb(BackgroundHex);
        Surface0 = Color.FromArgb(Surface0Hex);
        Surface1 = Color.FromArgb(Surface1Hex);
        Surface2 = Color.FromArgb(Surface2Hex);
        Surface3 = Color.FromArgb(Surface3Hex);
        OnBackground = Color.FromArgb(OnBackgroundHex);
        OnSurface = Color.FromArgb(OnSurfaceHex);
        OnPrimary = Color.FromArgb(OnPrimaryHex);
        OnSecondary = Color.FromArgb(OnSecondaryHex);
        Error = Color.FromArgb(ErrorHex);
        Success = Color.FromArgb(SuccessHex);
        Info = Color.FromArgb(InfoHex);
        
        // Dark theme colors
        PrimaryDark = Color.FromArgb(PrimaryDarkHex);
        SecondaryDark = Color.FromArgb(SecondaryDarkHex);
        BackgroundDark = Color.FromArgb(BackgroundDarkHex);
        Surface0Dark = Color.FromArgb(Surface0DarkHex);
        Surface1Dark = Color.FromArgb(Surface1DarkHex);
        Surface2Dark = Color.FromArgb(Surface2DarkHex);
        Surface3Dark = Color.FromArgb(Surface3DarkHex);
        OnBackgroundDark = Color.FromArgb(OnBackgroundDarkHex);
        OnSurfaceDark = Color.FromArgb(OnSurfaceDarkHex);
        OnPrimaryDark = Color.FromArgb(OnPrimaryDarkHex);
        OnSecondaryDark = Color.FromArgb(OnSecondaryDarkHex);
        ErrorDark = Color.FromArgb(ErrorDarkHex);
        SuccessDark = Color.FromArgb(SuccessDarkHex);
        InfoDark = Color.FromArgb(InfoDarkHex);

    }
    
    /// <summary>
    /// Updates all hex string properties based on their Color counterparts
    /// </summary>
    public void UpdateHexFromColors()
    {
        // Light theme color hex strings
        PrimaryHex = Primary.ToHex();
        SecondaryHex = Secondary.ToHex();
        BackgroundHex = Background.ToHex();
        Surface0Hex = Surface0.ToHex();
        Surface1Hex = Surface1.ToHex();
        Surface2Hex = Surface2.ToHex();
        Surface3Hex = Surface3.ToHex();
        OnBackgroundHex = OnBackground.ToHex();
        OnSurfaceHex = OnSurface.ToHex();
        OnPrimaryHex = OnPrimary.ToHex();
        OnSecondaryHex = OnSecondary.ToHex();
        ErrorHex = Error.ToHex();
        SuccessHex = Success.ToHex();
        InfoHex = Info.ToHex();
        
        // Dark theme color hex strings
        PrimaryDarkHex = PrimaryDark.ToHex();
        SecondaryDarkHex = SecondaryDark.ToHex();
        BackgroundDarkHex = BackgroundDark.ToHex();
        Surface0DarkHex = Surface0Dark.ToHex();
        Surface1DarkHex = Surface1Dark.ToHex();
        Surface2DarkHex = Surface2Dark.ToHex();
        Surface3DarkHex = Surface3Dark.ToHex();
        OnBackgroundDarkHex = OnBackgroundDark.ToHex();
        OnSurfaceDarkHex = OnSurfaceDark.ToHex();
        OnPrimaryDarkHex = OnPrimaryDark.ToHex();
        OnSecondaryDarkHex = OnSecondaryDark.ToHex();
        ErrorDarkHex = ErrorDark.ToHex();
        SuccessDarkHex = SuccessDark.ToHex();
        InfoDarkHex = InfoDark.ToHex();
    }

    // Light theme colors - Hex strings for serialization
    public string PrimaryHex { get; set; }
    public string SecondaryHex { get; set; }
    public string BackgroundHex { get; set; }
    public string Surface0Hex { get; set; }
    public string Surface1Hex { get; set; }
    public string Surface2Hex { get; set; }
    public string Surface3Hex { get; set; }
    public string OnBackgroundHex { get; set; }
    public string OnSurfaceHex { get; set; }
    public string OnPrimaryHex { get; set; }
    public string OnSecondaryHex { get; set; }
    public string ErrorHex { get; set; }
    public string SuccessHex { get; set; }
    public string InfoHex { get; set; }

    // Dark theme colors - Hex strings for serialization
    public string PrimaryDarkHex { get; set; }
    public string SecondaryDarkHex { get; set; }
    public string BackgroundDarkHex { get; set; }
    public string Surface0DarkHex { get; set; }
    public string Surface1DarkHex { get; set; }
    public string Surface2DarkHex { get; set; }
    public string Surface3DarkHex { get; set; }
    public string OnBackgroundDarkHex { get; set; }
    public string OnSurfaceDarkHex { get; set; }
    public string OnPrimaryDarkHex { get; set; }
    public string OnSecondaryDarkHex { get; set; }
    public string ErrorDarkHex { get; set; }
    public string SuccessDarkHex { get; set; }
    public string InfoDarkHex { get; set; }

    // Title property
    public string Title { get; set; } = string.Empty;

    // Light theme colors - Ignored during JSON serialization
    [JsonIgnore]
    public Color Background { get; set; }
    [JsonIgnore]
    public Color Surface0 { get; set; }
    [JsonIgnore]
    public Color Surface1 { get; set; }
    [JsonIgnore]
    public Color Surface2 { get; set; }
    [JsonIgnore]
    public Color Surface3 { get; set; }
    [JsonIgnore]
    public Color Primary { get; set; }
    [JsonIgnore]
    public Color OnPrimary { get; set; }
    [JsonIgnore]
    public Color Secondary { get; set; }
    [JsonIgnore]
    public Color OnSecondary { get; set; }
    [JsonIgnore]
    public Color Error { get; set; }
    [JsonIgnore]
    public Color Success { get; set; }
    [JsonIgnore]
    public Color Info { get; set; }
    [JsonIgnore]
    public Color OnBackground { get; set; }
    [JsonIgnore]
    public Color OnSurface { get; set; }
    

    // Dark theme colors - Ignored during JSON serialization
    [JsonIgnore]
    public Color BackgroundDark { get; set; }
    [JsonIgnore]
    public Color Surface0Dark { get; set; }
    [JsonIgnore]
    public Color Surface1Dark { get; set; }
    [JsonIgnore]
    public Color Surface2Dark { get; set; }
    [JsonIgnore]
    public Color Surface3Dark { get; set; }
    [JsonIgnore]
    public Color PrimaryDark { get; set; }
    [JsonIgnore]
    public Color OnPrimaryDark { get; set; }
    [JsonIgnore]
    public Color SecondaryDark { get; set; }
    [JsonIgnore]
    public Color OnSecondaryDark { get; set; }
    [JsonIgnore]
    public Color ErrorDark { get; set; }
    [JsonIgnore]
    public Color SuccessDark { get; set; }
    [JsonIgnore]
    public Color InfoDark { get; set; }
    [JsonIgnore]
    public Color OnBackgroundDark { get; set; }
    [JsonIgnore]
    public Color OnSurfaceDark { get; set; }
    
}