# RazorSoft.Core  
## Configuration
____________________________________________________________________________________________________  
[Home][1] | [Whiteboard][2]

We revive a settings & configuration staple with some modifications. These functions provide us with 
a manageable settings library - easy to use and maintain. New features should *never* break compatibility 
with older settings files. This initial version of the library is considered the basic, default behavior.

By default, setting files will be save with the `.config` extension. The file is written in **JSON**.  

One difference is that the value is serialized. The library needs to have some updates that allows 
a plain, human readable section so users can make changes without special utility intervention.

Sample settings from unit testing:  
```json

{
  "key0": "CC01C7C18F41D808"
}

```

Ref:  
    + [Whiteboard][2]: feature suggestions  
    + [Unit tests][3]: in-depth use and implementation  
____________________________________________________________________________________________________   
© 2020 RazorSoft Media, DBA  
       Lone Star Logistics & Transport, LLC. All Rights Reserved  

[1]: ../../README.md
[2]: ../whiteboard.md
[3]: ../testing/Test.RazorSoft.Core/ConfigurationTests.cs
