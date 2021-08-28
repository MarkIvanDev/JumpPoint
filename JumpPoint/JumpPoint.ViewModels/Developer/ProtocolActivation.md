# Protocol Activation

#### 1. jumppoint://
> Opens a new Jump Point window  


#### 2. jumppoint://`path`
> Opens the specified Jump Point **path** in a new window.  
> Accepted `path` values:  
> `dashboard, settings, favorites, workspaces, drives, applinks, settinglinks`  


#### 3. jumppoint://`item`/?path=`path`
> Opens the Jump Point **item** with the specified **path** in a new window  
  
Parameter|Valid Values|Required|Description
:---|:---|:---|:---
item|drive, folder, workspace|Yes|The item type to view
path|--string--|Yes|The path to the item that you want to view


#### 4. jumppoint://properties/?type=`type`&path=`path`
> Opens the Jump Point Properties window for the specified **type** and **path**  
  
Parameter|Valid Values|Required|Description
:---|:---|:---|:---
type|drive, folder, file, workspace, settinglink, applink|Yes|The type of the item that will be opened in the Jump Point Properties window
path|--string--|Yes|The path to the item that you want to view


#### 5. jumppoint://properties/?fileToken=`fileToken`
> Opens the Jump Point Properties window for the file that the **fileToken** points to  

Parameter|Valid Values|Required|Description
:---|:---|:---|:---
fileToken|--string--|Yes|A token identifying the file that will be opened in the new Jump Point properties window. Retrieved from [SharedStorageAccessManager.AddFile](https://docs.microsoft.com/en-us/uwp/api/windows.applicationmodel.datatransfer.sharedstorageaccessmanager.addfile)


#### 6. jumppoint://properties/?seedsToken=`seedsToken`
> Opens the Jump Point Properties window for the contents the file that the **seedsToken** points to

Parameter|Valid Values|Required|Description
:---|:---|:---|:---
seedsToken|--string--|Yes|A token identifying the file that contains the serialized JSON collection of Seeds that will be opened in the new Jump Point properties window. Retrieved from [SharedStorageAccessManager.AddFile](https://docs.microsoft.com/en-us/uwp/api/windows.applicationmodel.datatransfer.sharedstorageaccessmanager.addfile)

##### JSON Schema  
Type: Array of Objects  
Object Properties: **type** (string), **path** (string)  
See Table in Section 4 to see the valid values for **type** and **path**

###### Example
```
    [
    	{"type":"drive","path":"C:\\\\"},
    	{"type":"folder","path":"C:\\\\Windows"},
    	{"type":"file","path":"C:\\\\file.txt"},  
    	{"type":"workspace","path":"Music"},
    	{"type":"settinglink","path":"home"},
    	{"type":"applink","path":"jumppoint://dashboard"}
    ]
```