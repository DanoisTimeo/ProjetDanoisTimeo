using System.Net.Http.Json;

namespace ProjetDanoisTimeo
{
    public partial class MainPage : ContentPage
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnSearchButtonClicked(object sender, EventArgs e)
        {
            string prenom = PrenomEntry.Text;
            if (int.TryParse(AnneeEntry.Text, out int annee) && annee >= 2003 && annee <= DateTime.Now.Year)
            {
                try
                {
                    var result = await GetPrenomCount(prenom, annee);
                    ResultLabel.Text = $"Nombre d'enfants nés avec le prénom {prenom} depuis {annee} : {result}";
                }
                catch (Exception ex)
                {
                    ResultLabel.Text = $"Erreur : {ex.Message}";
                }
            }
            else
            {
                ResultLabel.Text = "Veuillez entrer une année valide entre 2003 et aujourd'hui.";
            }
        }

        private async Task<int> GetPrenomCount(string prenom, int annee)
        {
            string url = $"https://data.nantesmetropole.fr/api/explore/v2.1/catalog/datasets/244400404_prenoms-enfants-nes-nantes/records?select=count(*)&where=enfant_prenom%3D%27{prenom}%27%20and%20annee%3D{annee}&group_by=commune_nom&limit=20";
            var response = await httpClient.GetFromJsonAsync<ApiResponse>(url);
            var result = response.Results.FirstOrDefault();
            return result != null ? result.Count : 0; // Probleme result toujours null ? 
        }
    }

    public class ApiResponse
    {
        public Result[] Results { get; set; }
    }

    public class Result
    {
        public string CommuneNom { get; set; }
        public int Count { get; set; }
    }
}
