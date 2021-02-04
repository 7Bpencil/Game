## Hotline Miami Clone
Small game, that greatly inspired by Hotline Miami. 

It was my first attempt to write a program that bigger than 200 LOC.  
The game is written from scratch (except FMOD audio engine) on .NET Framework.  
I was inexperienced and naive, so code is mostly dumb.  
But I am thankful that this project happened - it showed me what is what in game dev.

Click on picture to watch gameplay:

[![](pictures/preview.png)](https://www.youtube.com/watch?v=kpBl_tBHNdA "gameplay video")

### Build
    use "dotnet cake" command to build

### Controls
~~~
WASD - move character  
LMB - fire gun  
RMB - melee attack  
Mouse Scroll - change gun  
Left Shift - dash  
Press I to show debug info  
Press P to enable shadows (info below)  
~~~

### Shadows
We tried to add some shadows, but GDI+ cannot handle it, framerate drops significantly.
![](pictures/shadows.png "Shadows")


### License
Sounds are from random youtube videos.  
Some sprites I draw myself, some are from [Hotline Miami](https://store.steampowered.com/app/219150/Hotline_Miami/), some (like blood) are from [Brutal Doom](https://www.moddb.com/mods/brutal-doom).  
So I will leave it unlicensed.

### Participants:
- Vl4d1sl0veZ4r1p0v (taught me GDI+, wrote bot AI)
- 7Bpencil (everything else)
