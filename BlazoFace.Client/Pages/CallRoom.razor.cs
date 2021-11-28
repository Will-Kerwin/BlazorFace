using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace BlazoFace.Client.Pages
{
    public partial class CallRoom : IAsyncDisposable
    {

        private string RoomName = "";
        private HubConnection? hubCon;
        private IJSObjectReference? videoModule;
        private List<object> Users = new List<object>();

        [Inject] public IConfiguration? Configuration { get; set; }

        [Inject] public ILogger<CallRoom> Logger { get; set; }

        [Parameter] public Guid RoomId { get; set; }

        [Inject] public IJSRuntime JS { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                videoModule = await JS.InvokeAsync<IJSObjectReference>("import",
                "./video.js");
                await StartVideo();
            }
        }

        async Task StartVideo()
        {
            if (videoModule is not null)
                await videoModule.InvokeVoidAsync("startVideo");
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            RoomName = RoomId.ToString().Substring(0, 5);
            if (hubCon is not null)
            {
                await hubCon.SendAsync("joinRoom", RoomId);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            hubCon = new HubConnectionBuilder()
                .WithUrl(new Uri(Configuration!["VideoHubURI"]))
                .Build();

            hubCon.On<string, Guid>("userConnected", async (message, roomId) =>
             {
                 Logger.LogInformation(message);
                 using var streamRef = new DotNetStreamReference(stream: null, leaveOpen: false);
                 if(videoModule is not null)
                    await videoModule.InvokeVoidAsync("addUserStream", streamRef);
                 StateHasChanged();
             });

            hubCon.On<string, Guid>("userDisconnected", (message, roomId) =>
            {
                Logger.LogInformation(message);
                StateHasChanged();
            });

            await hubCon.StartAsync();
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (videoModule is not null)
            {
                await videoModule.DisposeAsync();
            }
            if (hubCon is not null)
            {
                await hubCon.DisposeAsync();
            }
        }
    }
}
