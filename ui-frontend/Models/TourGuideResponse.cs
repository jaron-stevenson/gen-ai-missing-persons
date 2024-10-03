using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ui_frontend.Models
{
    public class TourGuideResponse
    {
        public string? Message { get; set; }
        //public MetaData? MetaData { get; set; }
        public List<Trip>? Destinations { get; set; }
    }
    public class MetaData
    {
        public int CompletionTokens { get; set; }
        public int PromptTokens { get; set; }
        public int TotalTokens { get; set; }
    }
    public class Trip
    {
        public string? Destination { get; set; }
        [JsonPropertyName("starting_point")]
        public string? StartingPoint { get; set; }
        public string? Summary { get; set; }
        [JsonPropertyName("estimated_drive_time")]
        public string? EstimatedDriveTime { get; set; }
        public List<Activity>? Activities { get; set; }
        public List<Accommodation>? Accommodations { get; set; }
    }

    public class Activity
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
    }
    public class Accommodation
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
    }
}
