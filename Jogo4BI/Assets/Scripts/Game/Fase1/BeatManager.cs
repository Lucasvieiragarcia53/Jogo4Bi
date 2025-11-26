using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;

public class BeatManager : MonoBehaviour
{
   
    [SerializeField] private float _bpm; // Batidas por minuto da música
    [SerializeField] private AudioSource _audioSource; // Fonte de áudio para tocar a música
    [SerializeField] private Intervals[] _intervals; // Array de intervalos para triggers baseados em beats

    // Novos campos para sistema de pontuação e combo
    [SerializeField] private int _score; // Pontuação total do jogador
    [SerializeField] private int _combo; // Contador de combo
    [SerializeField] private TMP_Text _scoreText; 
    [SerializeField] private TMP_Text _comboText; 
    [SerializeField] private TMP_Text _hitQualityText;


    [SerializeField] private float _perfectTolerance = 0.05f;
    [SerializeField] private float _goodTolerance = 0.1f;    
    [SerializeField] private float _okTolerance = 0.2f;       

    private float _currentBeat; // Beat atual calculado baseado no tempo da música

    public enum HitQuality { Miss, Ok, Good, Perfect }

    //acessar o beat atual de fora da classe
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
        // Calcula o beat atual baseado nos samples de áudio e BPM
        //beat = (tempo em samples / frequência) / (60 / BPM)
        _currentBeat = (_audioSource.timeSamples / (_audioSource.clip.frequency * (60f / _bpm)));

        // Verifica cada intervalo definido para triggers
        foreach (Intervals interval in _intervals)
        {
            // Calcula o tempo amostrado para o intervalo específico
            float sampledTime = (_audioSource.timeSamples / (_audioSource.clip.frequency * interval.GetIntervalLength(_bpm)));
            interval.CheckForIntervals(sampledTime); // Verifica se deve disparar o evento
        }

        // Se a música terminou, carrega a próxima cena
        if (_audioSource != null && !_audioSource.isPlaying)
        {
            SceneManager.LoadScene("Fase 2");
        }
    }

    // Método para obter a qualidade do hit atual
    public HitQuality GetHitQuality()
    {
        // Calcula a fração do beat atual
        float beatFraction = _currentBeat - Mathf.Floor(_currentBeat);
        // Distância ao beat mais próximo
        float distanceToBeat = Mathf.Min(beatFraction, 1f - beatFraction);

        // Determina a qualidade baseada nas tolerâncias
        if (distanceToBeat <= _perfectTolerance)
            return HitQuality.Perfect;
        else if (distanceToBeat <= _goodTolerance)
            return HitQuality.Good;
        else if (distanceToBeat <= _okTolerance)
            return HitQuality.Ok;
        else
            return HitQuality.Miss;
    }

    // Método para verificar se está no ritmo
    public bool IsOnBeat()
    {
        return GetHitQuality() != HitQuality.Miss;
    }

    // Adiciona pontos à pontuação baseada na qualidade do hit
    public void AddScore(HitQuality quality)
    {
        int basePoints = 0; // Pontos base dependendo da qualidade
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

        // Adiciona pontos multiplicados pelo combo
        _score += basePoints * (_combo > 0 ? _combo : 1);
        UpdateUI(); 
        ShowHitQuality(quality); 
    }

    // Incrementa o combo (chamado em hits bem-sucedidos)
    public void IncrementCombo()
    {
        _combo++;
        UpdateUI();
    }

    // Reseta o combo
    public void ResetCombo()
    {
        _combo = 0;
        UpdateUI();
    }

    // Retorna o tempo em segundos até o próximo beat
    public float GetTimeToNextBeat()
    {
        float beatLength = 60f / _bpm; // Duração de um beat em segundos
        float currentTime = _audioSource.time; // Tempo atual da música
        // Calcula o tempo do próximo beat
        float nextBeatTime = Mathf.Ceil(currentTime / beatLength) * beatLength;
        return nextBeatTime - currentTime; // Tempo restante
    }

    // Atualiza os textos da UI com pontuação e combo atuais
    private void UpdateUI()
    {
        if (_scoreText != null)
        {
            _scoreText.text = "Pontos: " + _score.ToString();
        }
        if (_comboText != null)
        {
            _comboText.text = "Combo: " + _combo.ToString();
        }
    }

    // Exibe a qualidade do hit na UI por um curto período
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
            // Inicia corrotina para esconder o texto
            StartCoroutine(HideHitQualityText());
        }
    }

    // Corrotina para esconder o texto da qualidade
    private IEnumerator HideHitQualityText()
    {
        yield return new WaitForSeconds(1f);
        if (_hitQualityText != null)
            _hitQualityText.text = "";
    }

    // Classe para definir intervalos de triggers baseados em beats
    [System.Serializable]
    public class Intervals
    {
        [SerializeField] private float _steps; // Número de passos por beat
        [SerializeField] private UnityEvent _trigger; // Evento Unity a ser disparado no intervalo
        private int _LastInterval; // Último intervalo verificado

        // Calcula o comprimento do intervalo em segundos baseado no BPM
        public float GetIntervalLength(float bpm)
        {
            return 60f / (bpm * _steps);
        }

        // Verifica se o intervalo mudou e dispara o evento se necessário
        public void CheckForIntervals(float interval)
        {
            if (Mathf.FloorToInt(interval) != _LastInterval)
            {
                _LastInterval = Mathf.FloorToInt(interval);
                _trigger.Invoke(); // Dispara o evento Unity
            }
        }
    }
}
