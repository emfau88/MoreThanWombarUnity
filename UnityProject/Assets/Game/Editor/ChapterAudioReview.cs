using System;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace WombatLab.Editor
{
    // Bounded capture of Unity's actual output; no replacement sound track.
    public static class ChapterAudioReview
    {
        static readonly List<float> samples = new List<float>();
        static double deadline;
        static bool recording;
        public static string Start()
        {
            if (!EditorApplication.isPlaying) throw new InvalidOperationException("Start Play first.");
            Stop(); samples.Clear();
            if (!AudioRenderer.Start()) throw new InvalidOperationException("Audio output is already recording.");
            recording = true; deadline = EditorApplication.timeSinceStartup + 12;
            EditorApplication.update += Tick; AssemblyReloadEvents.beforeAssemblyReload += Stop;
            return "Actual output capture in a bounded 12-second window started; audio output restored on completion.";
        }
        static void Tick()
        {
            if (!EditorApplication.isPlaying || EditorApplication.timeSinceStartup > deadline) { Stop(); return; }
            int count = AudioRenderer.GetSampleCountForCaptureFrame();
            if (count <= 0) return;
            using (var buffer = new NativeArray<float>(count * 2, Allocator.Temp))
                if (AudioRenderer.Render(buffer)) samples.AddRange(buffer.ToArray());
        }
        public static void Stop()
        {
            EditorApplication.update -= Tick; AssemblyReloadEvents.beforeAssemblyReload -= Stop;
            if (!recording) return;
            AudioRenderer.Stop(); recording = false;
            string folder = Path.Combine(Application.dataPath,"QA"); Directory.CreateDirectory(folder);
            float peak=0;double power=0;int clipped=0;
            using (var stream = File.Create(Path.Combine(folder,"b7-actual-audio.wav")))
            using (var writer = new BinaryWriter(stream))
            {
                int bytes=samples.Count*2, rate=AudioSettings.outputSampleRate;
                writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));writer.Write(36+bytes);
                writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt "));writer.Write(16);writer.Write((short)1);writer.Write((short)2);
                writer.Write(rate);writer.Write(rate*4);writer.Write((short)4);writer.Write((short)16);
                writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));writer.Write(bytes);
                foreach(float value in samples)
                {peak=Mathf.Max(peak,Mathf.Abs(value));power+=value*value;if(Mathf.Abs(value)>=1)clipped++;writer.Write((short)(Mathf.Clamp(value,-1,1)*32767));}
            }
            File.WriteAllText(Path.Combine(folder,"b7-audio-report.txt"),"Unity actual stereo output; capturedAudioSeconds="+(samples.Count/(2.0*AudioSettings.outputSampleRate))+"; samples="+samples.Count
                +" rate="+AudioSettings.outputSampleRate+" peak="+peak+" rms="+(samples.Count>0?Math.Sqrt(power/samples.Count):0)+" clipped="+clipped);
            samples.Clear();
        }
    }
}
