using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[Serializable]
public class MatchData
{
    public DateTime date;
    public string homeTeam;
    public string awayTeam;
    public int homeScore;
    public int awayScore;
    public string tournamentCategory;
    public string city;
    public string country;
}

public class MatchManager : MonoBehaviour
{
    [Header("CSV Data")]
    public TextAsset csvFile;

    [Header("Tournament Selection UI")]
    public TMP_Dropdown tournamentDropdown;

    [Header("Match List Panel UI (Match 1)")]
    public TextMeshProUGUI match1TeamA;
    public TextMeshProUGUI match1TeamB;
    public TextMeshProUGUI match1Date;
    public TextMeshProUGUI match1City;
    public TextMeshProUGUI match1Country;

    [Header("Match List Panel UI (Match 2)")]
    public TextMeshProUGUI match2TeamA;
    public TextMeshProUGUI match2TeamB;
    public TextMeshProUGUI match2Date;
    public TextMeshProUGUI match2City;
    public TextMeshProUGUI match2Country;

    [Header("Match List Panel UI (Match 3)")]
    public TextMeshProUGUI match3TeamA;
    public TextMeshProUGUI match3TeamB;
    public TextMeshProUGUI match3Date;
    public TextMeshProUGUI match3City;
    public TextMeshProUGUI match3Country;

    private List<MatchData> allMatches = new List<MatchData>();

    void Start()
    {
        LoadCSVData();
        PopulateTournamentDropdown();
    }

    void LoadCSVData()
    {
        if (csvFile == null)
        {
            Debug.LogError("CSV file not assigned in the inspector!");
            return;
        }

        string[] rows = csvFile.text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // Assume the first row is a header.
        for (int i = 1; i < rows.Length; i++)
        {
            string row = rows[i].Trim();
            if (string.IsNullOrEmpty(row))
                continue;

            string[] columns = row.Split(',');

            if (columns.Length < 8)
                continue;

            MatchData match = new MatchData();

            if (DateTime.TryParse(columns[0], out DateTime parsedDate))
                match.date = parsedDate;
            else
                match.date = DateTime.MinValue;

            match.homeTeam = columns[1].Trim();
            match.awayTeam = columns[2].Trim();

            match.homeScore = int.TryParse(columns[3].Trim(), out int homeScore) ? homeScore : 0;
            match.awayScore = int.TryParse(columns[4].Trim(), out int awayScore) ? awayScore : 0;

            match.tournamentCategory = columns[5].Trim();
            match.city = columns[6].Trim();
            match.country = columns[7].Trim();

            allMatches.Add(match);
        }

        Debug.Log("CSV Data Loaded. Total matches: " + allMatches.Count);
    }

    void PopulateTournamentDropdown()
    {
        // Extract unique tournament names from the CSV data
        List<string> tournaments = allMatches
            .Select(m => m.tournamentCategory)
            .Distinct()
            .OrderBy(name => name)
            .ToList();

        tournamentDropdown.ClearOptions();
        tournamentDropdown.AddOptions(tournaments);
        Debug.Log("Tournament dropdown populated with " + tournaments.Count + " options.");
    }

    public void OnChallengeButtonClicked()
    {
        string selectedTournament = tournamentDropdown.options[tournamentDropdown.value].text;
        Debug.Log("Selected Tournament: " + selectedTournament);

        List<MatchData> filteredMatches = allMatches
            .Where(m => m.tournamentCategory.Equals(selectedTournament, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (filteredMatches.Count == 0)
        {
            Debug.LogWarning("No matches available for tournament: " + selectedTournament);
            return;
        }

        // Create 3 match selections, allowing duplicates if less than 3 unique matches exist.
        List<MatchData> selectedMatches = new List<MatchData>();
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, filteredMatches.Count);
            selectedMatches.Add(filteredMatches[randomIndex]);
        }

        // Save the selected matches in the static holder.
        MatchDataHolder.SelectedMatches.Clear();
        MatchDataHolder.SelectedMatches.AddRange(selectedMatches);

        // Update the match panels in this scene.
        UpdateMatchPanel(match1TeamA, match1TeamB, match1Date, match1City, match1Country, selectedMatches[0]);
        UpdateMatchPanel(match2TeamA, match2TeamB, match2Date, match2City, match2Country, selectedMatches[1]);
        UpdateMatchPanel(match3TeamA, match3TeamB, match3Date, match3City, match3Country, selectedMatches[2]);
    }

    public void OnPlayButtonPressed()
    {
        SceneManager.LoadScene("MatchSimulationScene");
    }

    void UpdateMatchPanel(TextMeshProUGUI teamAText, TextMeshProUGUI teamBText, TextMeshProUGUI dateText, TextMeshProUGUI cityText, TextMeshProUGUI countryText, MatchData match)
    {
        teamAText.text = match.homeTeam;
        teamBText.text = match.awayTeam;
        dateText.text = match.date != DateTime.MinValue ? match.date.ToString("dd/MM/yyyy") : "N/A";
        cityText.text = match.city;
        countryText.text = match.country;
    }
}
