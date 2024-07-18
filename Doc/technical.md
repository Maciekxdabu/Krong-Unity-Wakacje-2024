# Technical Documentation

## Keep code readable

Follow basic unity style guides:
- https://unity.com/how-to/naming-and-code-style-tips-c-scripting-unity

Additional tips:

- OOP - don't expose public fields, provide public functions that create APIs
  - https://medium.com/@CodeWithHonor/c-encapsulation-6b59be896312
- DRY - SRP - keep functions and classes small 
  - separate long functions and classes to sereval smaller ones by finding independant and logical parts
  - when parts are small and cleanly defined it is easier to share code and prevent logic repetiton
  - https://refactoring.guru/smells/long-method
  - https://refactoring.guru/smells/large-class
- YAGNI / KISS - don't overcomplicate code without any particular examples that can be tested in forseeable future
  - https://www.techtarget.com/whatis/definition/You-arent-gonna-need-it
- Naming - spend time to create readable names
  - meaningful class names
  - function names, that tell what a function does
  - argument names
  - field names
  - https://itequia.com/en/the-art-of-clean-coding-the-power-of-meaningful-names/
- write comments where necesairy
  - https://swimm.io/learn/code-collaboration/comments-in-code-best-practices-and-mistakes-to-avoid
- write temporary comments as a good way to get more clarity on what your code is doing
  - see [Rubber Duck Debuigging](https://en.wikipedia.org/wiki/Rubber_duck_debugging)
