# ImageSlicerBack

Server side of image slicer writen on C# using ASP.NET framework with REST API.

Application receive image with parameters from client, and return pdf file with sliced image ready to print.

Client side writen on Angular https://github.com/michaelenoroexe/imageSlicerFront

## How it works

User from client send image file and required parameter to this application, 
and then it split image to needed size and split it to pieces of standart paper format requested from user, then it just send splitted image saved in pdf to user. 

## Receiving parameters

File - Image file that needed to split on parts.

type - Name of standart print paper type.

colNum - Number of columns, to split. 

orientation - Define orientation of pages, user need.

### Possible values of params:

type :

  Name  |  Size mm
 - A0  |  841, 1189
 - A1  |  594, 841
 - A2  |  420, 594
 - A3  |  297, 420
 - A4  |  210, 297
 - A5  |  148, 210
 - A6  |  105, 148

colNum - Maximum of colums are depends from selected format and dunamicly change on client

orientation - landscape or portrait

If size of image is not enough to fit user requested format type and column number, then application automatically stretch it.
Columns installed by the user on the outher hand number of rows calculates automatically.

## Purpose

You can use this application to make poster from image and print it.
