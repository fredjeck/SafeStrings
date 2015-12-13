#SafeStrings : Remove those plain text passwords in your configuration files.

**IMPORTANT NOTE**

**SafeStrings is intended to be used in a controlled environment, to safely obfuscate (thus strings are encrypted) some of your data.**
**Always keep in mind when using it from your code that .Net assemblies can be easily "decompiled" using tools such as [DotPeek](https://www.jetbrains.com/decompiler/)**

SafeStrings is a collection of the String class extension that allows you to safely encrypt and decrypt strings using a password.

The encryption is performed using AES and a 256bits length key derivated from the a user provided password.

The generated strings are totally autonomous, meaning that they contain everything (the IV and the Salt are stored along the data) needed to decrypt them (provided that you supply the good password).

SafeStrings has no dependencies and can be easily included in any project.

##Usage :
### From the code :
```
var enc = "This string should be encrypted".EncryptUsingPassword("ThisIsMySuperStringPassword");
```
Returns **x-enc:6wgbPm6FQZ/HB5d1gUEAWLQhadQ89kxyyXUD9p90wfzpcoOg39gmk9ItzkgsVZIkFOW4FrlfEbQ=**

```
var dec = "x-enc:6wgbPm6FQZ/HB5d1gUEAWLQhadQ89kxyyXUD9p90wfzpcoOg39gmk9ItzkgsVZIkFOW4FrlfEbQ=".DecryptUsingPassword("ThisIsMySuperStringPassword");
```
Returns **This string should be encrypted**

### From the command line using the SafeStrings executable :
```
> SafeStrings enc ""Short string to encode"" ""MyPassword123""
x-enc:vNFWG2xadwyApHLxZ9XHbCtf65Xl+HudgO6JxWWyt0S+5UeRiypoz/MIx6xVv3CIxD6cAjWlo6E=                                                                              

> SafeStrings d ""x-enc:vNFWG2xadwyApHLxZ9XHbCtf65Xl+HudgO6JxWWyt0S+5UeRiypoz/MIx6xVv3CIxD6cAjWlo6E="" ""MyPassword123""
Short string to encode
```
