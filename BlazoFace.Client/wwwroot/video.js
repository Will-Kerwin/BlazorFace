const vidGrid = document.getElementById('video-grid');
const newVideo = document.createElement('video');

export function startVideo() {
    newVideo.muted = true;

    navigator.mediaDevices.getUserMedia({
        video: true,
        audio: true
    }).then(stream => {
        addVideoStream(newVideo, stream);
    });
}

export async function addUserStream(streamRef) {
    const stream = await streamRef.stream();
}

export function addVideoStream(video, stream) {
    video.srcObject = stream;
    video.addEventListener('loadedmetadata', () => {
        video.play();
    });
    vidGrid.append(video);
}