# Unity-Easy-LoadingScreen

A simple loading screen system for Unity that supports loading multiple scenes at a time additively.
Can be called from any scene without needing a specific manager component. The only setup required is to reference the loading screen prefab from the data Scriptable Object.

https://user-images.githubusercontent.com/17034238/144736576-8c59be97-2b81-4118-90b3-e464f5189157.mp4

## Setup

Ensure your first scene contains the SceneLoaderManager component somewhere. Make the Loadscreen UI a child of this so that it does not get destroyed.

## Making your own loading screen

Loading Screen prefab must have a SceneLoaderUI component on it's top-level. Actual progress bar is optional but is set to an Image and uses the fill mode. The rest of the loading screen variables are set here as follows:

`Canvas Group`: Canvas Group handles fading the loading screen in and out.

`Loading Screen Fade Time`: How long it takes the loading screen to fade in or out.

`Min Time To Load`: The loading screen will not "finish" until this at least amount of time has passed.

`Progress Bar`: An optional image used for progress. Must be set to Image Type: Filled.

`Progress Update Speed`: How fast the progress bar moves to new values.

`Progress`: Allows for setting the progress level manually via inspector.

## Usage

Call `SceneLoaderManager.LoadScene` or `SceneLoaderManager.LoadScenes` to start the loading process.

Parameters are as follows:

### LoadScene

`(string) Scene Name` : Name of the scene to load.

`(bool) Is Additive` : Whether to load this new scene additively.

`(Action) On Loaded Callback` : Will be called when everything is done loading.

### LoadScenes

`(string[]) Scene Name` : Array containing the string names of the scenes.

`(bool) Is Additive` : Whether to load these new scenes additively. If false, the first scene will load as a Single, but the rest will be additive.

`(Action) On Loaded Callback` : Will be called when everything is done loading.

## Dependancies
Demigiant's DOTween - http://dotween.demigiant.com/
