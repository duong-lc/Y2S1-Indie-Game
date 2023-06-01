using System;

namespace Core.Events {
    public enum EventType {
        TestEvent = 0,
        LogEvent,
        CompileDataFromMidiEvent,
        //OnNoteHit
        NoteHitNoneEvent, //to fill in param
        NoteHitEarlyEvent,
        NoteHitPerfectEvent,
        NoteHitLateEvent,
        NoteMissEvent,
        
        UnPauseEvent,
        RemoveSliderFromHoldListEvent,
        SpawnNoteNormal,
        SpawnNoteSlider,
        
        SliderNoteHoldingEvent,
        SliderNoteReleaseEvent,
        
        StartSongEvent,
        LaneFinishSpawningEvent,
        PauseTransitionEvent,
        GameEndedEvent
    }
}