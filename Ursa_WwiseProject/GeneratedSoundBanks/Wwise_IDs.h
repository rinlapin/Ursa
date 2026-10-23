/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID GAMEPLAY = 89505537U;
        static const AkUniqueID MAIN_MENU_MUSIC = 3399698792U;
        static const AkUniqueID OPENING_CUTSCENE = 1917576450U;
        static const AkUniqueID PLAY_CONST_MOVE = 3094787361U;
        static const AkUniqueID PLAY_CONST_REBIRTH = 4101259872U;
        static const AkUniqueID PLAY_FOOTSTEPS = 3854155799U;
        static const AkUniqueID PLAY_ICE_SMASH = 2850134090U;
        static const AkUniqueID PLAY_NAV_STAR_DRONE = 3425158555U;
        static const AkUniqueID PLAY_PUZZLE_HINT_MASTER = 3416524667U;
        static const AkUniqueID PLAY_PUZZLE_WIN_MASTER = 206505562U;
        static const AkUniqueID PLAY_WIND_AMB = 2232409101U;
        static const AkUniqueID STOP_FOOTSTEPS = 2963349357U;
        static const AkUniqueID STOP_NAV_STAR_DRONE = 3266201885U;
        static const AkUniqueID TO_BE_CONTINUED = 1170557302U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace GAMEPLAY
        {
            static const AkUniqueID GROUP = 89505537U;

            namespace STATE
            {
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID PUZZLE_SOLVING = 3258781332U;
                static const AkUniqueID TALKING_TO_ARIES = 4227414720U;
                static const AkUniqueID WANDERING = 342806866U;
            } // namespace STATE
        } // namespace GAMEPLAY

        namespace MAIN_MENU
        {
            static const AkUniqueID GROUP = 2005704188U;

            namespace STATE
            {
                static const AkUniqueID MENU_EXIT = 3663878895U;
                static const AkUniqueID MENU_OPEN = 1287511387U;
                static const AkUniqueID NONE = 748895195U;
            } // namespace STATE
        } // namespace MAIN_MENU

    } // namespace STATES

    namespace SWITCHES
    {
        namespace PLAYER_LOCATION
        {
            static const AkUniqueID GROUP = 3238956108U;

            namespace SWITCH
            {
                static const AkUniqueID IN_THE_CAVE = 1403589664U;
                static const AkUniqueID OUTSIDE = 438105790U;
            } // namespace SWITCH
        } // namespace PLAYER_LOCATION

    } // namespace SWITCHES

    namespace BUSSES
    {
        static const AkUniqueID MAIN_AUDIO_BUS = 2246998526U;
    } // namespace BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
