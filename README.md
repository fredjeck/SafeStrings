# SafeStrings : Remove those plain text passwords in your configuration files.
SafeStrings is a collection of the String class extension that allows you to safely encrypt and decrypt strings using a password.

The encryption is performed using AES and a 256bits length key derivated from the supplied password.

The generated strings are totally autonomous, meaning that they contain everything (the IV and the Salt are stored along the data) needed to decrypt them (provided that you supply the good password).
