using Microsoft.JSInterop;

namespace BlazorSsrDynamic.Components;

public class JsConsoleLogger
{
    private readonly IJSRuntime _jsRuntime;
        
    public JsConsoleLogger(IJSRuntime jSRuntime)
    {
        _jsRuntime = jSRuntime;
    }

    public async Task LogAsync(object message)
    {
        await _jsRuntime.InvokeVoidAsync("console.log", message);
    }
}