using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;

public class BeatManager : MonoBehaviour
{

    [SerializeField] private float _bpm;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Intervals[] _intervals;

    // Novos campos para pontuação e combo
    [SerializeField] private int _score;
    [SerializeField] private int _combo;
    [SerializeField] private TMP_Text _scoreText; // Ou Text se não usar TMP
    [SerializeField] private TMP_Text _comboText; // Ou Text se não usar TMP
    [SerializeField] private TMP_Text _hitQualityText; // Novo: Texto para mostrar a qualidade do hit

    // Tolerâncias para diferentes qualidades de hit (ajuste conforme necessário)
    [SerializeField] private float _perfectTolerance = 0.05f; // Muito preciso
    [SerializeField] private float _goodTolerance = 0.1f;     // Médio
    [SerializeField] private float _okTolerance = 0.2f;       // Largo

    private float _currentBeat;

    // Enum para qualidade do hit
    public enum HitQuality { Miss, Ok, Good, Perfect }

    // Novo: Getter para _currentBeat
    public float GetCurrentBeat()
    {
        return _currentBeat;
    }

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        // Calcular o beat atual baseado no tempo da música
        _currentBeat = (_audioSource.timeSamples / (_audioSource.clip.frequency * (60f / _bpm)));

        foreach (Intervals interval in _intervals)
        {
            float sampledTime = (_audioSource.timeSamples / (_audioSource.clip.frequency * interval.GetIntervalLength(_bpm)));
            interval.CheckForIntervals(sampledTime);
        }

        if (_audioSource != null && !_audioSource.isPlaying)
        {
            LoadNextLevel();
        }
    }

    // Modificado: Retorna a qualidade do hit em vez de apenas bool
    public HitQuality GetHitQuality()
    {
        float beatFraction = _currentBeat - Mathf.Floor(_currentBeat);
        float distanceToBeat = Mathf.Min(beatFraction, 1f - beatFraction); // Distância ao beat mais próximo

        if (distanceToBeat <= _perfectTolerance)
            return HitQuality.Perfect;
        else if (distanceToBeat <= _goodTolerance)
            return HitQuality.Good;
        else if (distanceToBeat <= _okTolerance)
            return HitQuality.Ok;
        else
            return HitQuality.Miss;
    }

    // Método para verificar se uma ação está "no ritmo" (mantido para compatibilidade, mas use GetHitQuality())
    public bool IsOnBeat()
    {
        return GetHitQuality() != HitQuality.Miss;
    }

    // Modificado: Adicionar pontos baseado na qualidade do hit
    public void AddScore(HitQuality quality)
    {
        int basePoints = 0;
        switch (quality)
        {
            case HitQuality.Perfect:
                basePoints = 300;
                break;
            case HitQuality.Good:
                basePoints = 200;
                break;
            case HitQuality.Ok:
                basePoints = 100;
                break;
            case HitQuality.Miss:
                basePoints = 0;
                break;
        }

        _score += basePoints * (_combo > 0 ? _combo : 1); // Multiplica pelo combo, mínimo 1
        UpdateUI();
        ShowHitQuality(quality); // Mostra a qualidade na UI
    }

    // Método para incrementar o combo
    public void IncrementCombo()
    {
        _combo++;
        UpdateUI();
    }

    // Método para resetar o combo (quando erra)
    public void ResetCombo()
    {
        _combo = 0;
        UpdateUI();
    }

    // Novo: Retorna o tempo em segundos até o próximo beat
    public float GetTimeToNextBeat()
    {
        float beatLength = 60f / _bpm; // Tempo por beat em segundos
        float currentTime = _audioSource.time; // Tempo atual da música
        float nextBeatTime = Mathf.Ceil(currentTime / beatLength) * beatLength; // Próximo beat
        return nextBeatTime - currentTime; // Tempo restante
    }

    // Atualizar a UI do placar
    private void UpdateUI()
    {
        if (_scoreText != null)
            _scoreText.text = "Pontos: " + _score.ToString();
        if (_comboText != null)
            _comboText.text = "Combo: " + _combo.ToString();
    }

    // Novo: Mostrar a qualidade do hit na UI
    private void ShowHitQuality(HitQuality quality)
    {
        if (_hitQualityText != null)
        {
            string qualityText = "";
            switch (quality)
            {
                case HitQuality.Perfect:
                    qualityText = "Ótimo!";
                    break;
                case HitQuality.Good:
                    qualityText = "Bom!";
                    break;
                case HitQuality.Ok:
                    qualityText = "Ok!";
                    break;
                case HitQuality.Miss:
                    qualityText = "Errou!";
                    break;
            }
            _hitQualityText.text = qualityText;
            // Opcional: Esconder após alguns segundos
            StartCoroutine(HideHitQualityText());
        }
    }

    // Corrotina para esconder o texto de qualidade após 1 segundo
    private IEnumerator HideHitQualityText()
    {
        yield return new WaitForSeconds(1f);
        if (_hitQualityText != null)
            _hitQualityText.text = "";
    }

    void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
    }


    [System.Serializable]
    public class Intervals
    {
        [SerializeField] private float _steps;
        [SerializeField] private UnityEvent _trigger;
        private int _LastInterval;

        public float GetIntervalLength(float bpm)
        {
            return 60f / (bpm * _steps);
        }

        public void CheckForIntervals(float interval)
        {
            if (Mathf.FloorToInt(interval) != _LastInterval)
            {
                _LastInterval = Mathf.FloorToInt(interval);
                _trigger.Invoke();
            }
        }
    }
}