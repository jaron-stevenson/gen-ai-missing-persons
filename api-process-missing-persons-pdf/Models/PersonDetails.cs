/*
{
  "name": "",
  "race": "",
  "age": 0,
  "sex": "",
  "height": "",
  "weight": "",
  "eye_color": "",
  "hair": "",
  "alias": "",
  "tattoos": "",
  "last_seen": "",
  "date_reported": "",
  "missing_from": "",
  "conditions_of_disappearance": "",
  "officer_details": {
    "name": "",
    "badge_number": "",
    "department": ""
  },
  "phone": ""
}
*/

using System.Text.Json.Serialization;

namespace api_process_mp_pdfs.Models;
public class MissingPerson
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("race")]
    public string? Race { get; set; }

    [JsonPropertyName("age")]
    public int Age { get; set; }

    [JsonPropertyName("sex")]
    public string? Sex { get; set; }

    [JsonPropertyName("height")]
    public string? Height { get; set; }

    [JsonPropertyName("weight")]
    public string? Weight { get; set; }

    [JsonPropertyName("eye_color")]
    public string? EyeColor { get; set; }

    [JsonPropertyName("hair")]
    public string? Hair { get; set; }

    [JsonPropertyName("alias")]
    public string? Alias { get; set; }

    [JsonPropertyName("tattoos")]
    public string? Tattoos { get; set; }

    [JsonPropertyName("last_seen")]
    public string? LastSeen { get; set; }

    [JsonPropertyName("date_reported")]
    public string? DateReported { get; set; }

    [JsonPropertyName("missing_from")]
    public string? MissingFrom { get; set; }

    [JsonPropertyName("conditions_of_disappearance")]
    public string? ConditionsOfDisappearance { get; set; }

    [JsonPropertyName("officer_details")]
    public OfficerDetails? OfficerDetails { get; set; }

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }
}

public class OfficerDetails
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("badge_number")]
    public string? BadgeNumber { get; set; }

    [JsonPropertyName("department")]
    public string? Department { get; set; }
}
