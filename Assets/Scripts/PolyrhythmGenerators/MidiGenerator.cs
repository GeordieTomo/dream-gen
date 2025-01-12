using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MidiGenerator : MonoBehaviour
{

    private MidiFile midiFile;
    private bool cancel = false;

    public void Start ()
    {
        midiFile = new MidiFile ();
    }

    public IEnumerator GenerateMidi(string filePath, List<int> scaleDegrees, List<double> bpms, List<int> chordNumbers, double chordBPM, double startTime, double endTime, Slider progressBar, TMP_Text completionText, PendulumGenerator pendulumGen)
    {
        yield return CreateMidiFile(scaleDegrees, bpms, chordNumbers, chordBPM, startTime, endTime, progressBar, completionText, pendulumGen);
        midiFile.Write(filePath, true);
        yield return null;
    }

    private IEnumerator CreateMidiFile(List<int> scaleDegrees, List<double> bpms, List<int> chordNumbers, double chordBPM, double startTime, double endTime, Slider progressBar, TMP_Text completionText, PendulumGenerator pendulumGen)
    {
        cancel = false;

        progressBar.gameObject.SetActive(true);
        completionText.gameObject.SetActive(false);

        var patternBuilder = new PatternBuilder();
        var tempoMap = TempoMap.Default;

        if (startTime > endTime)
        {
            double tmp = endTime;
            endTime = startTime;
            startTime = tmp;
        }
        if (startTime == endTime)
        {
            Debug.LogError("Invalid range");
        }

        double startTimeNorm = startTime / 60 * bpms[bpms.Count - 1];
        double endTimeNorm = endTime / 60 * bpms[bpms.Count - 1];

        for (int i = 0; i < scaleDegrees.Count; i++)
        {
            double noteLength = bpms[bpms.Count - 1] / bpms[i];
            double firstNoteOccurance = noteLength - startTimeNorm % noteLength;
            if (startTimeNorm <= 0)
            {
                firstNoteOccurance -= noteLength;
            }

            int numberOfOccurances = Mathf.CeilToInt((float)((endTimeNorm - startTimeNorm - firstNoteOccurance) / noteLength));

            for (int j = 0; j < numberOfOccurances; j++)
            {
                // based on what chord we are up to, update the note number
                // chord index is time / chord period % num chords

                int currentChord;
                int numberOfChords = chordNumbers.Count;
                if (numberOfChords > 0)
                {
                    //double time = (startTimeNorm + noteLength * j) / (bpms[bpms.Count - 1] / 4) * chordBPM;
                    double time = (double)(startTimeNorm + noteLength * j) / (bpms[bpms.Count - 1]) * chordBPM;
                    int chordIndex = Mathf.FloorToInt(Convert.ToSingle(time)) % chordNumbers.Count;

                    if (chordIndex < 0)
                    {
                        chordIndex += numberOfChords;
                    }
                    currentChord = chordNumbers[chordIndex];
                }
                else
                {
                    currentChord = 1;
                }

                var noteNumber = Melanchall.DryWetMidi.MusicTheory.Note.Get(SevenBitNumber.Values[(pendulumGen.ScaleDegreesToSemitones(scaleDegrees[i], currentChord, false)) + 23]);

                var timeSpan = MusicalTimeSpan.FromDouble(noteLength);
                var length = MusicalTimeSpan.FromDouble(noteLength * 0.9);
                var startPoint = MusicalTimeSpan.FromDouble(firstNoteOccurance);
                patternBuilder.SetNoteLength(length)
                            .MoveToTime(startPoint + timeSpan * j)
                            .Note(noteNumber);
                float progress = (float)(i * numberOfOccurances + j) / (float)((scaleDegrees.Count+chordNumbers.Count) * (numberOfOccurances + 1));
                progressBar.value = progress;
                yield return null;
                if (cancel)
                    break;
            }

        }

        for (int i = 0; i < chordNumbers.Count; i++)
        {
            double noteLength = bpms[bpms.Count - 1] / chordBPM;
            double firstNoteOccurance = noteLength - startTimeNorm % noteLength;
            if (startTimeNorm <= 0)
            {
                firstNoteOccurance -= noteLength;
            }

            int numberOfOccurances = Mathf.CeilToInt((float)((endTimeNorm - startTimeNorm - firstNoteOccurance) / noteLength));

            for (int j = 0; j < numberOfOccurances; j++)
            {
                // based on what chord we are up to, update the note number
                // chord index is time / chord period % num chords

                int currentChord;
                int numberOfChords = chordNumbers.Count;
                if (numberOfChords > 0)
                {
                    //double time = (startTimeNorm + noteLength * j) / (bpms[bpms.Count - 1] / 4) * chordBPM;
                    double time = (double)(startTimeNorm + noteLength * j) / (bpms[bpms.Count - 1]) * chordBPM;
                    int chordIndex = Mathf.FloorToInt(Convert.ToSingle(time)) % chordNumbers.Count;

                    if (chordIndex < 0)
                    {
                        chordIndex += numberOfChords;
                    }
                    currentChord = chordNumbers[chordIndex];
                }
                else
                {
                    currentChord = 1;
                }

                var noteNumber = Melanchall.DryWetMidi.MusicTheory.Note.Get(SevenBitNumber.Values[(pendulumGen.ScaleDegreesToSemitones(currentChord, 1, true))+23]);


                var timeSpan = MusicalTimeSpan.FromDouble(noteLength);
                var length = MusicalTimeSpan.FromDouble(noteLength * 0.9);
                var startPoint = MusicalTimeSpan.FromDouble(firstNoteOccurance);
                patternBuilder.SetNoteLength(length)
                            .MoveToTime(startPoint + timeSpan * j)
                            .Note(noteNumber);
                float progress = (float)((i+ scaleDegrees.Count) * numberOfOccurances + j) / (float)((scaleDegrees.Count + chordNumbers.Count) * (numberOfOccurances + 1));
                progressBar.value = progress;
                yield return null;

                if (cancel)
                     break;
            }

        }

        if (cancel)
        {
            progressBar.gameObject.SetActive(false);
            completionText.gameObject.SetActive(false);
        }
        else
        {

            progressBar.value = 1;
            progressBar.gameObject.SetActive(false);
            completionText.gameObject.SetActive(true);

            midiFile = patternBuilder.Build().ToFile(tempoMap);
        }

        pendulumGen.MidiFileComplete();

        yield return null;

    }

    public void Cancel(Slider progressBar)
    {
        progressBar.gameObject.SetActive(false);
        cancel = true;
    }
}