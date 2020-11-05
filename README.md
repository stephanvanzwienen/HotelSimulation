# HotelSimulation
If you get a FileNotFoundException when referencing a new DLL in your project then do the following:
0. Never put the old and the new DLL side by side in a directory
1. Delete the old DLL from where you referenced it
2. Delete a copy of the DLL from the directory where the .exe is located (if copy local is enabled)
3. Delete the reference in your project (should now have a yellow triangle)
4. Apply a "Clean Solution" to your solution
5. Put the new DLL in the directory you want to reference it from
6. Rename the DLL (get the sequence number out)

Some steps will not be necessary for everyone, but if you follow these steps he should always do it again.