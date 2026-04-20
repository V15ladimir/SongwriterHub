using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace Songwriter.Services {

    public class MidiGenerationService : IMidiGenerationService {
        private const int QuarterNoteTicks = 480;
        private readonly Dictionary<int, string[]> _progressions = new() {
            { 0, new[] { "C", "G", "Am", "F" } },
            { 1, new[] { "Am", "F", "C", "G" } },
            { 2, new[] { "C", "Am", "F", "G" } },
            { 3, new[] { "G", "D", "Em", "C" } },
            { 4, new[] { "Em", "C", "G", "D" } },
            { 5, new[] { "F", "G", "Em", "Am" } },
        };

        public byte[] GenerateComposition(int id, ulong seed) {
            var songSeed = seed ^ (ulong)id;
            var randomSounds = new XoshiroRandomAdapter(songSeed);
            var progressionIndex = randomSounds.Next(_progressions.Count);
            var verseChords = _progressions[progressionIndex];
            var chorusIndex = (progressionIndex + 1 + randomSounds.Next(2)) % _progressions.Count;
            var chorusChords = _progressions[chorusIndex];
            var tempo = 300 + randomSounds.Next(30);
            var tempoMap = TempoMap.Create(Tempo.FromBeatsPerMinute(tempo));
            var midiFile = new MidiFile();
            midiFile.Chunks.Add(CreatePianoTrack(randomSounds, verseChords, 4, 0, 0));
            midiFile.Chunks.Add(CreatePianoTrack(randomSounds, chorusChords, 4, 1, QuarterNoteTicks * 16));
            midiFile.Chunks.Add(CreateBassTrack(randomSounds, verseChords, 4, 2, 0));
            midiFile.Chunks.Add(CreateBassTrack(randomSounds, chorusChords, 4, 3, QuarterNoteTicks * 16));
            midiFile.ReplaceTempoMap(tempoMap);
            using var stream = new MemoryStream();
            midiFile.Write(stream);
            return stream.ToArray();
        }

        private TrackChunk CreatePianoTrack(Random rnd, string[] chords, int repeats, int channel, long startOffset) {
            var track = new TrackChunk();
            var currentTime = startOffset;
            track.Events.Add(new ProgramChangeEvent((SevenBitNumber)0) {
                DeltaTime = currentTime,
                Channel = (FourBitNumber)channel
            });
            currentTime = 0;
            for(var repeat = 0; repeat < repeats; repeat++) {
                foreach(var chord in chords) {
                    var notes = GetChordNotes(chord);
                    var velocity = 70 + rnd.Next(20);
                    var notesToPlay = notes;
                    if(rnd.Next(4) == 0 && notes.Length > 2) {
                        notesToPlay = [notes[0], notes[2]];
                    }
                    foreach(var note in notesToPlay) {
                        track.Events.Add(new NoteOnEvent((SevenBitNumber)note, (SevenBitNumber)velocity) {
                            DeltaTime = currentTime,
                            Channel = (FourBitNumber)channel
                        });
                        track.Events.Add(new NoteOffEvent((SevenBitNumber)note, (SevenBitNumber)0) {
                            DeltaTime = QuarterNoteTicks,
                            Channel = (FourBitNumber)channel
                        });
                        currentTime = 0;
                    }
                    currentTime = QuarterNoteTicks / (2 + rnd.Next(2));
                }
            }
            return track;
        }

        private TrackChunk CreateBassTrack(Random rnd, string[] chords, int repeats, int channel, long startOffset) {
            var track = new TrackChunk();
            var currentTime = startOffset;
            track.Events.Add(new ProgramChangeEvent((SevenBitNumber)33) {
                DeltaTime = currentTime,
                Channel = (FourBitNumber)channel
            });
            currentTime = 0;
            for(var repeat = 0; repeat < repeats; repeat++) {
                foreach(var chord in chords) {
                    int bassNote;
                    if(rnd.Next(4) == 0) {
                        var notes = GetChordNotes(chord);
                        bassNote = notes[rnd.Next(notes.Length)] - 12;
                    } else {
                        bassNote = GetChordRoot(chord) - 12;
                    }
                    var velocity = 80 + rnd.Next(30);
                    track.Events.Add(new NoteOnEvent((SevenBitNumber)bassNote, (SevenBitNumber)velocity) {
                        DeltaTime = currentTime,
                        Channel = (FourBitNumber)channel
                    });
                    track.Events.Add(new NoteOffEvent((SevenBitNumber)bassNote, (SevenBitNumber)0) {
                        DeltaTime = QuarterNoteTicks,
                        Channel = (FourBitNumber)channel
                    });
                    currentTime = 0;
                    currentTime = QuarterNoteTicks / (2 + rnd.Next(2));
                }
            }
            return track;
        }

        private int[] GetChordNotes(string chord) {
            var root = GetChordRoot(chord);
            var isMinor = chord.EndsWith("m");
            if(isMinor) {
                return [root, root + 3, root + 7];
            } else {
                return [root, root + 4, root + 7];
            }
        }

        private int GetChordRoot(string chord) {
            var baseChord = chord
                .Replace("m", "")
                .Replace("7", "")
                .Replace("maj7", "");

            return baseChord switch {
                "C" => 60,
                "C#" => 61,
                "Db" => 61,
                "D" => 62,
                "D#" => 63,
                "Eb" => 63,
                "E" => 64,
                "F" => 65,
                "F#" => 66,
                "Gb" => 66,
                "G" => 67,
                "G#" => 68,
                "Ab" => 68,
                "A" => 69,
                "A#" => 70,
                "Bb" => 70,
                "B" => 71,
                _ => 60
            };
        }
    }
}
