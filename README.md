# Claude is love, Claude is life

## What is this nonsnse

A way to play around with Claude, handwrite an MCP server, and have it interact with an API i control

## How?

### Starting up

In [PersonAPI](./PersonAPI) run `dotnet run` (Launches on :5078)

In [MCP](./MCP) run `dotnet run` (Launches on :9000)

To add to claude:

```
claude mcp add --transport http <think of a good name> https://localhost:9000
```

### Working With it

Once you've fired up claude, ask it something

> Using the <think of a good name> mcp server, list all people

> There's one person listed:
> Lucas — date of birth 1996-11-29 (id:0)