# What's Parsa-HtmlParser
this is a .NET library to parse HTML documen. it created to parse big HTML document more than 50 mb. it created to be fast, simple and easy to use as well.

[nuget](https://www.nuget.org/packages/Parsa-HtmlParser/)

## How to

there is `HtmlReader` to 'Read' a File and 'Parse' HTML document as `string` it will back a `HtmlDocument`.

there is an example
```html
<!DOCTYPE html>
<html>
<head>
<title>Page Title</title>
</head>
<body class="main">

<h1 id='title'>This is a Heading</h1>
<p id="line1">This is a paragraph.</p>
<p id="line2">This is a paragraph too.</p>

</body>
</html>
```
```C#
[Fact]
public void Parse(){
    var doc = HtmlReader.Parse("the code above");
    //or
    //var doc = HtmlReader.Read("c:\\file.html");
    var element1 = doc[".main"];
    var element2 = doc["#title"];
    var element3 = doc["p"];
    var element4 = doc["div"]

    Assert.AreTrue(element1.Count == 1 && element1.TagName == "body");
    Assert.AreTrue(element1.TagName == "h1");
    Assert.AreTrue(element1.Count == 2);
    Assert.AreTrue(element1.Count == 0);
}
```
This example explain how to get elements by `class`, `id` or `tag`

## Use Linq

```C#
 var p2 = doc["p"].First(node => node.Id == "line2");
 var paragraphs = doc["P"].Select(node => node.InnerText);
```

## `HtmlNode` and `HtmlContent`
The `HtmlNode` is base, so html models extended from that.
The `HtmlContent` is array of `HtmlNode`. if it has onty one then it will behaves like `HtmlNode` but it still is an array.

```C#
var paragraphs = doc["p"]; //it is a HtmlContent that has two nodes inside
var body = doc["body"];  //it is a HtmlContent with one node
var bodyNode = doc.Content.First(node => node.TagName == "body")  //it is a HtmlNode that has Content as HtmlContent 

Assert.AreEqual(body.Id, bodyNode.Id);
Assert.AreEqual(paragraphs.Id, null);
```
With `HtmlContent` you can use Linq But with `HtmlNode` you can not
```c#
var body = doc["body"];
var title = doc["title"].Select(node => node.InnerText).First(); //it will be "Page Title"
//doc is HtmlDocument and it extented from HtmlNode so you can not use doc.Select(...)
//but you can
var bodyText = body.Select(node => node.InnerText);
```

## License
[MIT]()
