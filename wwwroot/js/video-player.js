// Video Player Manager
const VideoPlayer = {
    player: null,

    init() {
        this.player = document.getElementById('player');
    },

    play(path) {
   if (!this.player) {
     this.init();
   }

    if (!this.player) {
            console.error('Video player element not found');
  return;
        }

  // Stop and reset current video
        this.player.pause();
 this.player.currentTime = 0;

        // Load and play new video
 this.player.src = path;
        this.player.style.display = 'block';
   this.player.play();

  // Scroll to player
        window.scrollTo({ top: 0, behavior: 'smooth' });
    },

    stop() {
if (this.player) {
      this.player.pause();
   this.player.currentTime = 0;
     this.player.style.display = 'none';
        }
    }
};

// Initialize when DOM is ready
if (document.readyState === 'loading') {
document.addEventListener('DOMContentLoaded', () => VideoPlayer.init());
} else {
    VideoPlayer.init();
}
