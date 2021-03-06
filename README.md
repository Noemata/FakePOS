# Fake POS (purchase order system)

POC of porting a fictitious purchase order system to WinUI.  The code used is representative of a typical corporate UWP app.  The other similar snapshot made by Microsoft is not as representative of the evolving nature of real-world code, warts and all.

The port does attempt to rationalize aspects of the original code.  Updates to newer tech were made where needed (MVVM framework).

Description: 

This project demonstrates how a fictitious POS could be ported to WinUI while maintaining the same code concurrently with UWP as would be the case in a real-world migration.

UWP is used to facilitate overcoming the current authoring and deployment limitations of WinUI.  RAD (rapid application development) is made possible through framework enhancements shown in this solution.

Note how most of the WinUI solution contains nothing, with the bulk of the code being linked directly from the UWP project.  That is a good thing!

I have removed the purchase order processing screen, since that is part of a separate effort that is copyrighted.
The Charting screen has also been removed since it does not materially affect the porting effort and was changed significantly relative to the original code.
The Microsoft.Toolkit.Uwp.UI.Controls.DataGrid was used to replace Telerik's UWP version.  They are not directly interchangeable so that portion of this code still needs work as it represents a complete break from the original.

## Screenshots
![Screenshot](https://github.com/Noemata/FakePOS/blob/master/LoginView.png)

![Screenshot](https://github.com/Noemata/FakePOS/blob/master/Catalog.png)

## License same as original

https://github.com/dotnet-architecture/eShopOnUWP
https://github.com/Alvaromah/eShopOnUWP

## Credits and Ideas

https://github.com/dotnet-architecture/eShopOnUWP
https://github.com/Alvaromah/eShopOnUWP
https://github.com/microsoft/WinUI-3-Demos/tree/master/src/ContosoAirlinePOS
https://github.com/Sergio0694/Brainf_ckSharp
https://github.com/Noemata/SimpleMVVM

## Caveats

The conditional namespace shake and bake is perhaps needless.

The positioning of WinUI as a good UI alternative or next step to UWP UI capabilities is very premature with the qualifier that it does bring needed new capabilities to desktop apps needing easy Win32 integration.  UWP can be pacakged to deliver everything WinUI is trying to do and then some, but with complicated separation of concerns.

Performance is understandably lack luster given there's lots of under the hood tuning still needed, but we're not really there yet for production level considerations that need to be snappy.

There is still too much irrational churn with the framework's capabilities and feature set for WinUI to be considered for large apps.  Small utility apps on the other hand can already gain the benefits of WinUI.

Microsoft needs to embrace easy wins and avoid self inflicted wounds.  Dropping the Pivot control is a good example of the latter.

## Notes

The original port to WinUI 3 Preview 4 was done over (4) 7.5 hour days and went fairly smoothly.  The transition was impressively easy. A significant chunk of the time was spent making the original code more MVVM friendly, a task somewhat unrelated to porting though it ultimately simplified the porting process. Composition animation code had to be removed from WinUI.  Look for MP! in the comments for other changes or limitations.

Transitioning to Reunion 0.5 took just over an hour.  However, there were new issues.  C++ exceptions have to be disabled when debugging.

One portion of code is no longer functional: https://github.com/Noemata/FakePOS/blob/master/FakePOS/FakePOSuwp/Views/Catalog/ItemDetail/ItemDetailView.xaml
because it uses the Pivot control, formerly available in WinUI 3 Preview 4.

Microsoft will need to add back the Pivot control in WinUI 0.5, it's too common a UI pattern in UWP apps to be left out at this stage.

This sample will be updated once Microsoft adds back the Pivot control or someone else decides to author a substitute as not having Pivot will break many UWP porting efforts.

Porting UWP to WinUI could be made even easier with something like this: https://github.com/microsoft/microsoft-ui-xaml/issues/4445

Would be great to see this code get pushed over the finish line with the help of others (hint).
