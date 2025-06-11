/*
 * SUMMARY: This file contains the EngineSound class. 
 *          The purpose of this file is to handle the engine sounds when the engine is turned on.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RemoteEducation.Audio;
using RemoteEducation.Scenarios;
using RemoteEducation.Interactions;
using System;

[DisallowMultipleComponent]
/// <summary>Defines the behaviour of how the sounds should change when the engine is running and where the user is located.</summary>
public sealed class EngineSoundController : MonoBehaviour
{
    /// <summary> Positions of where the player can be in correlation to sounds</summary>
    public enum Position
    {
        None,
        Front,
        Back,
        Inside
    }

    [Tooltip("Ignition Key Sound Effect.")]
    [SerializeField] private SoundEffect ignitionKeySoundEffect;

    [Tooltip("Ignition Sound Effect.")]
    [SerializeField] private SoundEffect ignitionSoundEffect;

    [Tooltip("Ignition Sound Duration.")]
    [SerializeField] private float ignitionDuration = 1f;

    [Tooltip("Shutdown Sound Effect.")]
    [SerializeField] private SoundEffect shutdownSoundEffect;

    [Tooltip("Shutdown Sound Duration.")]
    [SerializeField] private float shutdownDuration = 1f;

    [Tooltip("Inside Cab Engine Sound.")]
    [SerializeField] private SoundEffect insideSoundEffect;

    [Tooltip("Front Side Engine Sound.")]
    [SerializeField] private SoundEffect frontSoundEffect;

    [Tooltip("Back Side Sound.")]
    [SerializeField] private SoundEffect backSoundEffect;

    [Tooltip("Key Off Sound Effect.")]
    [SerializeField] private SoundEffect keyOffSoundEffect;

    [Tooltip("Sound Now Playing.")]
    [SerializeField] private SoundEffect currentEngineSound;

    /// <summary>Camera reference for locations</summary>
    private CoreCameraController POICamera => CoreCameraController.Instance;

    private bool IsEngineRunning = false;

    private Position currentPosition;

    /// <summary>This function only plays the beep sound.</summary>
    public void PlayBeep()
    {
        ignitionKeySoundEffect.Play();
    }

    /// <summary>This function only plays the sound of a key turning off, since we don't have one I used a sound from
    /// the lights turning off from a different project.</summary>
    public void PlayKeyOff()
    {
        keyOffSoundEffect.Play();
    }

    /// <summary>This function starts the engine.</summary>
    public void StartEngine()
    {
        StartCoroutine(StartEngineLoop());
    }

    /// <summary>This function stops the engine.</summary>
    public void StopEngine()
    {
        StartCoroutine(StopEngineLoop());
    }

    /// <summary>Starting up the engine sounds.</summary>
    IEnumerator StartEngineLoop()
    {
        PlayBeep();
        yield return new WaitForSeconds(ignitionDuration/2);
        ignitionSoundEffect.Volume = 0.2f;
        ignitionSoundEffect.Play();
        yield return new WaitForSeconds(ignitionDuration);
        ignitionSoundEffect.FadeOut(0.5f);
        PlaySound(Position.Inside);
        IsEngineRunning = true;
        currentPosition = Position.Inside;
    }

    /// <summary>Stopping the engine sounds.</summary>
    IEnumerator StopEngineLoop()
    {
        shutdownSoundEffect.Play();
        currentEngineSound?.FadeOut(shutdownDuration);
        yield return new WaitForSeconds(shutdownDuration);
        IsEngineRunning = false;
        currentPosition = Position.None;
        currentEngineSound = null;
    }

    /// <summary>The function that changes the sound perspectives.</summary>
    public void PlaySound(Position position)
    {
        if(currentPosition == position)
            return;

        if(currentEngineSound != null)
            currentEngineSound.FadeOut(shutdownDuration / 4);

        switch (position)
        {
            case Position.Inside:
                currentPosition = Position.Inside;
                currentEngineSound = insideSoundEffect;
                insideSoundEffect.Play(true);
                break;
            case Position.Front:
                currentPosition = Position.Front;
                currentEngineSound = frontSoundEffect;
                frontSoundEffect.Play(true);
                break;
            case Position.Back:
                currentPosition = Position.Back;
                currentEngineSound = backSoundEffect;
                backSoundEffect.Play(true);
                break;
        }
    }

    /// <summary>This function triggers on POI changes and changes the played track based on POI positions.</summary>
    public void ChangeEngineSoundPosition(Position newPos)
    {
        if (!IsEngineRunning)
            return;

        PlaySound(newPos);
    }
}