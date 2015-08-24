# Parser

**Facebook message parser.**

Facebook allows to download whole account data from your account, including whole messaging history. That appears they store conversations which are unaccessable via messenger (if user is blocked / deleted). This tool allows to parse messages.htm file where all those messages are stored. It effectively converts all messages and conversations to entities allowing it to be queried.

Uses HtmlAgilityPack to parse HTM markup, Entity Framework to store and query messages.


Features: 
  - Converts .htm message database to entities
  - Queries to get interesting statistics
  - Chat export to .txt

Plans:
  - Make a working parser library
  - Graphs
  - More statistics
  - Better user interface
