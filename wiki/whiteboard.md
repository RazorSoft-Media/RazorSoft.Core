# RazorSoft.Core
## Core library: utility & function  
____________________________________________________________________________________________________  
[Home][1] | [Whiteboard][2]

### The Whiteboard
For features, suggestions, etc. If an item is taken up for implementation, create an issue and remove 
from this page.

* **Configuration**:
   - Settings Sections
   - Section Attributes: Readable, Serialized, Encoded, Encrypted
   - Settings Utility: to create & edit settings files
   
* **Messaging**:  
   - EventPublisher
     - Asynchronous publications: defaults event publishing to asynchronous distribution
	 - PublishAsync method: publishes specifically to asynchronous distribution
   - CommandRouter
     - Add Command API routing allow to `void` return
	 - ExecuteAsync method: execute commands async (with `await` and `await Task<T>`

____________________________________________________________________________________________________   
© 2020 RazorSoft Media, DBA  
       Lone Star Logistics & Transport, LLC. All Rights Reserved  
       
[1]: ../README.md
[2]: whiteboard.md
