using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchSimulationManager : MonoBehaviour
{
    [Header("Match Preview Panels (one for each match)")]
    public GameObject matchPanel1;
    public GameObject matchPanel2;
    public GameObject matchPanel3;

    [Header("Match Preview Panel UI Components")]
    // Preview Panel 1
    public TextMeshProUGUI preview1TeamA;
    public TextMeshProUGUI preview1TeamB;
    public TextMeshProUGUI preview1Date;
    public TextMeshProUGUI preview1City;
    public TextMeshProUGUI preview1Country;
    // Preview Panel 2
    public TextMeshProUGUI preview2TeamA;
    public TextMeshProUGUI preview2TeamB;
    public TextMeshProUGUI preview2Date;
    public TextMeshProUGUI preview2City;
    public TextMeshProUGUI preview2Country;
    // Preview Panel 3
    public TextMeshProUGUI preview3TeamA;
    public TextMeshProUGUI preview3TeamB;
    public TextMeshProUGUI preview3Date;
    public TextMeshProUGUI preview3City;
    public TextMeshProUGUI preview3Country;

    [Header("In-Game HUD (During match play)")]
    public GameObject matchHUD;             // Contains team names, scores, and timer
    public TextMeshProUGUI hudTeamAName;      // Displays team A name (copied from preview)
    public TextMeshProUGUI hudTeamBName;      // Displays team B name (copied from preview)
    public TextMeshProUGUI hudTeamAScore;     // Displays team A score
    public TextMeshProUGUI hudTeamBScore;     // Displays team B score
    public TextMeshProUGUI hudTimerText;      // Displays remaining time

    [Header("Final Scoreboard Panel (Always Active)")]
    // Score Matches Panel (Simulation Results)
    [Header("Score Matches Panel")]
    public TextMeshProUGUI scoreMatch1TeamANameText;
    public TextMeshProUGUI scoreMatch1TeamAScoreText;
    public TextMeshProUGUI scoreMatch1TeamBNameText;
    public TextMeshProUGUI scoreMatch1TeamBScoreText;

    public TextMeshProUGUI scoreMatch2TeamANameText;
    public TextMeshProUGUI scoreMatch2TeamAScoreText;
    public TextMeshProUGUI scoreMatch2TeamBNameText;
    public TextMeshProUGUI scoreMatch2TeamBScoreText;

    public TextMeshProUGUI scoreMatch3TeamANameText;
    public TextMeshProUGUI scoreMatch3TeamAScoreText;
    public TextMeshProUGUI scoreMatch3TeamBNameText;
    public TextMeshProUGUI scoreMatch3TeamBScoreText;

    // Data Historique Panel (Original CSV Data)
    [Header("Data Historique Panel")]
    public TextMeshProUGUI dataHistoriqueMatch1TeamANameText;
    public TextMeshProUGUI dataHistoriqueMatch1TeamAScoreText;
    public TextMeshProUGUI dataHistoriqueMatch1TeamBNameText;
    public TextMeshProUGUI dataHistoriqueMatch1TeamBScoreText;

    public TextMeshProUGUI dataHistoriqueMatch2TeamANameText;
    public TextMeshProUGUI dataHistoriqueMatch2TeamBNameText;
    public TextMeshProUGUI dataHistoriqueMatch2TeamAScoreText;
    public TextMeshProUGUI dataHistoriqueMatch2TeamBScoreText;

    public TextMeshProUGUI dataHistoriqueMatch3TeamANameText;
    public TextMeshProUGUI dataHistoriqueMatch3TeamAScoreText;
    public TextMeshProUGUI dataHistoriqueMatch3TeamBNameText;
    public TextMeshProUGUI dataHistoriqueMatch3TeamBScoreText;

    // Points Panel - Each match has its own TMP field, plus one for the total.
    [Header("Points Panel")]
    public TextMeshProUGUI pointsMatch1Text;
    public TextMeshProUGUI pointsMatch2Text;
    public TextMeshProUGUI pointsMatch3Text;
    public TextMeshProUGUI totalPointsText;

    [Header("Scoreboard Panel")]
    public GameObject scoreboardPanel;  // The overall final scoreboard panel.

    [Header("Gameplay Controllers & Football Object")]
    public MonoBehaviour playerController1;   // First player controller script
    public MonoBehaviour playerController2;   // Second player controller script
    public GameObject footballGameObject;     // Football GameObject

    [Header("Default Transforms (for resetting positions)")]
    public Transform player1DefaultTransform;
    public Transform player2DefaultTransform;
    public Transform ballDefaultTransform;
    // References to the actual player GameObjects to reset their positions:
    public GameObject player1;
    public GameObject player2;

    [Header("Timing Settings")]
    public float previewDuration = 3f;        // Duration (in seconds) for match preview panels
    public float matchDuration = 30f;         // Duration (in seconds) for match play phase

    // List of match data passed from the previous scene via the static holder.
    private List<MatchData> selectedMatches;

    // Variables to track the current match scores (simulation results).
    private int currentMatchTeamAScore;
    private int currentMatchTeamBScore;

    // List to store simulation results for each match.
    private List<MatchResult> matchResults = new List<MatchResult>();

    // Flag to prevent multiple resets at once.
    private bool isResetting = false;

    void Start()
    {
        // Retrieve match data from the static holder.
        selectedMatches = MatchDataHolder.SelectedMatches;
        if (selectedMatches == null || selectedMatches.Count < 3)
        {
            Debug.LogError("Not enough matches available in MatchDataHolder!");
            return;
        }

        // Initially disable HUD, and disable gameplay controllers and football.
        matchHUD.SetActive(false);
        playerController1.enabled = false;
        playerController2.enabled = false;
        footballGameObject.SetActive(false);
        scoreboardPanel.SetActive(false);  // Initially hidden

        // Start the simulation sequence.
        StartCoroutine(RunMatchesSequence());
    }

    IEnumerator RunMatchesSequence()
    {
        // Loop for each of the three matches.
        for (int i = 0; i < 3; i++)
        {
            // 1. Before showing preview, reset positions.
            ResetPositions();

            // 2. Update and show the preview panel for the current match.
            UpdatePreviewPanel(i);
            GameObject currentMatchPanel = GetMatchPanel(i);
            if (currentMatchPanel != null)
            {
                currentMatchPanel.SetActive(true);
                // Disable controllers and football during preview.
                playerController1.enabled = false;
                playerController2.enabled = false;
                footballGameObject.SetActive(false);
                yield return new WaitForSeconds(previewDuration);
                currentMatchPanel.SetActive(false);
            }

            // 3. Copy preview data to HUD team name texts.
            MatchData match = selectedMatches[i];
            hudTeamAName.text = match.homeTeam;
            hudTeamBName.text = match.awayTeam;

            // Initialize HUD scores.
            hudTeamAScore.text = "0";
            hudTeamBScore.text = "0";
            currentMatchTeamAScore = 0;
            currentMatchTeamBScore = 0;

            // Show the HUD.
            matchHUD.SetActive(true);

            // Enable gameplay: player controllers and football.
            playerController1.enabled = true;
            playerController2.enabled = true;
            footballGameObject.SetActive(true);

            // 4. Run the match play phase for the specified duration.
            float timeRemaining = matchDuration;
            while (timeRemaining > 0)
            {
                hudTimerText.text = Mathf.Ceil(timeRemaining).ToString();
                yield return null;
                // Only count down the timer if the player movement is enabled.
                if (playerController1.enabled)
                {
                    timeRemaining -= Time.deltaTime;
                }
            }

            // End of match play: disable gameplay and hide the HUD.
            playerController1.enabled = false;
            playerController2.enabled = false;
            footballGameObject.SetActive(false);
            matchHUD.SetActive(false);

            // Save the simulation result.
            matchResults.Add(new MatchResult(match.homeTeam, match.awayTeam, currentMatchTeamAScore, currentMatchTeamBScore));

            // Optional short delay before next match.
            yield return new WaitForSeconds(1f);
        }

        // After all matches, disable gameplay and show the final scoreboard.
        DisplayScoreboard();
    }

    // Updates the preview panel UI for the match at the given index.
    void UpdatePreviewPanel(int index)
    {
        MatchData match = selectedMatches[index];
        switch (index)
        {
            case 0:
                if (preview1TeamA != null) preview1TeamA.text = match.homeTeam;
                if (preview1TeamB != null) preview1TeamB.text = match.awayTeam;
                if (preview1Date != null) preview1Date.text = (match.date != DateTime.MinValue) ? match.date.ToString("dd/MM/yyyy") : "N/A";
                if (preview1City != null) preview1City.text = match.city;
                if (preview1Country != null) preview1Country.text = match.country;
                break;
            case 1:
                if (preview2TeamA != null) preview2TeamA.text = match.homeTeam;
                if (preview2TeamB != null) preview2TeamB.text = match.awayTeam;
                if (preview2Date != null) preview2Date.text = (match.date != DateTime.MinValue) ? match.date.ToString("dd/MM/yyyy") : "N/A";
                if (preview2City != null) preview2City.text = match.city;
                if (preview2Country != null) preview2Country.text = match.country;
                break;
            case 2:
                if (preview3TeamA != null) preview3TeamA.text = match.homeTeam;
                if (preview3TeamB != null) preview3TeamB.text = match.awayTeam;
                if (preview3Date != null) preview3Date.text = (match.date != DateTime.MinValue) ? match.date.ToString("dd/MM/yyyy") : "N/A";
                if (preview3City != null) preview3City.text = match.city;
                if (preview3Country != null) preview3Country.text = match.country;
                break;
            default:
                break;
        }
    }

    // Returns the correct match preview panel based on index.
    GameObject GetMatchPanel(int index)
    {
        switch (index)
        {
            case 0: return matchPanel1;
            case 1: return matchPanel2;
            case 2: return matchPanel3;
            default: return null;
        }
    }

    // Resets the positions of players and the football.
    void ResetPositions()
    {
        // Reset player1.
        if (player1 != null && player1DefaultTransform != null)
        {
            player1.transform.SetPositionAndRotation(player1DefaultTransform.position, player1DefaultTransform.rotation);
            Rigidbody rb1 = player1.GetComponent<Rigidbody>();
            if (rb1 != null)
            {
                rb1.velocity = Vector3.zero;
                rb1.angularVelocity = Vector3.zero;
            }
        }
        // Reset player2.
        if (player2 != null && player2DefaultTransform != null)
        {
            player2.transform.SetPositionAndRotation(player2DefaultTransform.position, player2DefaultTransform.rotation);
            Rigidbody rb2 = player2.GetComponent<Rigidbody>();
            if (rb2 != null)
            {
                rb2.velocity = Vector3.zero;
                rb2.angularVelocity = Vector3.zero;
            }
        }
        // Reset football.
        if (footballGameObject != null && ballDefaultTransform != null)
        {
            footballGameObject.transform.SetPositionAndRotation(ballDefaultTransform.position, ballDefaultTransform.rotation);
            Rigidbody rbBall = footballGameObject.GetComponent<Rigidbody>();
            if (rbBall != null)
            {
                rbBall.velocity = Vector3.zero;
                rbBall.angularVelocity = Vector3.zero;
            }
        }
    }

    void OnEnable()
    {
        GoalZone.OnGoalScored += OnGoalScored;
    }

    void OnDisable()
    {
        GoalZone.OnGoalScored -= OnGoalScored;
    }

    // When a goal is scored, update the simulation score and trigger a reset after a 1-second pause.
    void OnGoalScored(int side)
    {
        if (!matchHUD.activeSelf)
            return;

        if (side == 1)
        {
            currentMatchTeamAScore++;
            hudTeamAScore.text = currentMatchTeamAScore.ToString();
        }
        else if (side == 0)
        {
            currentMatchTeamBScore++;
            hudTeamBScore.text = currentMatchTeamBScore.ToString();
        }

        if (!isResetting)
            StartCoroutine(ResetAfterGoal());
    }

    // Pauses for 1 second, resets positions, then re-enables gameplay if needed.
    IEnumerator ResetAfterGoal()
    {
        isResetting = true;
        playerController1.enabled = false;
        playerController2.enabled = false;
        footballGameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        ResetPositions();
        if (matchHUD.activeSelf)
        {
            playerController1.enabled = true;
            playerController2.enabled = true;
            footballGameObject.SetActive(true);
        }
        isResetting = false;
    }

    // Displays the final scoreboard and enables the scoreboard panel.
    void DisplayScoreboard()
    {
        // Disable gameplay.
        playerController1.enabled = false;
        playerController2.enabled = false;
        footballGameObject.SetActive(false);

        // Enable the final scoreboard panel.
        scoreboardPanel.SetActive(true);

        int totalPoints = 0;
        int[] matchPoints = new int[3];

        // Process each match.
        if (matchResults.Count >= 3)
        {
            // Match 1.
            MatchResult sim1 = matchResults[0];
            MatchData orig1 = selectedMatches[0];
            matchPoints[0] = CalculatePoints(sim1, orig1);
            totalPoints += matchPoints[0];

            scoreMatch1TeamANameText.text = sim1.teamA;
            scoreMatch1TeamAScoreText.text = sim1.scoreA.ToString();
            scoreMatch1TeamBNameText.text = sim1.teamB;
            scoreMatch1TeamBScoreText.text = sim1.scoreB.ToString();

            dataHistoriqueMatch1TeamANameText.text = orig1.homeTeam;
            dataHistoriqueMatch1TeamAScoreText.text = orig1.homeScore.ToString();
            dataHistoriqueMatch1TeamBNameText.text = orig1.awayTeam;
            dataHistoriqueMatch1TeamBScoreText.text = orig1.awayScore.ToString();

            pointsMatch1Text.text = matchPoints[0].ToString();

            // Match 2.
            MatchResult sim2 = matchResults[1];
            MatchData orig2 = selectedMatches[1];
            matchPoints[1] = CalculatePoints(sim2, orig2);
            totalPoints += matchPoints[1];

            scoreMatch2TeamANameText.text = sim2.teamA;
            scoreMatch2TeamAScoreText.text = sim2.scoreA.ToString();
            scoreMatch2TeamBNameText.text = sim2.teamB;
            scoreMatch2TeamBScoreText.text = sim2.scoreB.ToString();

            dataHistoriqueMatch2TeamANameText.text = orig2.homeTeam;
            dataHistoriqueMatch2TeamAScoreText.text = orig2.homeScore.ToString();
            dataHistoriqueMatch2TeamBNameText.text = orig2.awayTeam;
            dataHistoriqueMatch2TeamBScoreText.text = orig2.awayScore.ToString();

            pointsMatch2Text.text = matchPoints[1].ToString();

            // Match 3.
            MatchResult sim3 = matchResults[2];
            MatchData orig3 = selectedMatches[2];
            matchPoints[2] = CalculatePoints(sim3, orig3);
            totalPoints += matchPoints[2];

            scoreMatch3TeamANameText.text = sim3.teamA;
            scoreMatch3TeamAScoreText.text = sim3.scoreA.ToString();
            scoreMatch3TeamBNameText.text = sim3.teamB;
            scoreMatch3TeamBScoreText.text = sim3.scoreB.ToString();

            dataHistoriqueMatch3TeamANameText.text = orig3.homeTeam;
            dataHistoriqueMatch3TeamAScoreText.text = orig3.homeScore.ToString();
            dataHistoriqueMatch3TeamBNameText.text = orig3.awayTeam;
            dataHistoriqueMatch3TeamBScoreText.text = orig3.awayScore.ToString();

            pointsMatch3Text.text = matchPoints[2].ToString();
        }

        totalPointsText.text = totalPoints.ToString();
    }

    // Calculates points for a match.
    // 3 points if the exact score matches,
    // 2 points if the winning team is correct (score differs),
    // 1 point if both are ties,
    // 0 points otherwise.
    int CalculatePoints(MatchResult sim, MatchData orig)
    {
        if (sim.scoreA == orig.homeScore && sim.scoreB == orig.awayScore)
        {
            return 3;
        }
        else if (sim.scoreA == sim.scoreB && orig.homeScore == orig.awayScore)
        {
            return 1;
        }
        else if ((sim.scoreA > sim.scoreB && orig.homeScore > orig.awayScore) ||
                 (sim.scoreA < sim.scoreB && orig.homeScore < orig.awayScore))
        {
            return 2;
        }
        else
        {
            return 0;
        }
    }

    public class MatchResult
    {
        public string teamA;
        public string teamB;
        public int scoreA;
        public int scoreB;

        public MatchResult(string teamA, string teamB, int scoreA, int scoreB)
        {
            this.teamA = teamA;
            this.teamB = teamB;
            this.scoreA = scoreA;
            this.scoreB = scoreB;
        }
    }
}
