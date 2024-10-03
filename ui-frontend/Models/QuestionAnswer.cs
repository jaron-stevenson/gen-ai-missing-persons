namespace ui_frontend.Models
{
    public class QuestionAnswer
    {
        public string? Question { get; set; }
        public string? Answer { get; set; }

        public bool? AnswerHidden { get; set; }

        public List<Trip> Trips
        {
            get
            {
                return _trips;
            }
        }

        private List<Trip> _trips { get; set; } = new List<Trip>();

        public QuestionAnswer(string question, string answer)
        {
            Question = question;
            Answer = answer;
        }

        public void AddTrip(Trip trip) => _trips.Add(trip);
        public void AddTrips(List<Trip> trips) => _trips.AddRange(trips);

    }
}
