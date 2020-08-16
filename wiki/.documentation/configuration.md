# RazorSoft.Core  
## Configuration
____________________________________________________________________________________________________  
[Home][1] | [Whiteboard][2]

We revive a settings & configuration staple with some modifications. These functions 
provide us with very modular configuration - that is, every library can have its own settings file. 
By default, setting files will be save with the `.config` extension. The file is written in **JSON**.  

One difference is that the value is serialized. The library needs to have some updates that allows 
a plain, human readable section so users can make changes without special utility intervention.

Sample settings from unit testing:  
```json

{
  "key0": "CC01C7C18F41D808"
}

```

May be useful to include other encoding or even encryption.

See the unit tests on how to use and implement.  
  
____________________________________________________________________________________________________   
© 2020 RazorSoft Media, DBA  
       Lone Star Logistics & Transport, LLC. All Rights Reserved  

[1]: ../../README.md
[2]: ../whiteboard.md
