$(document).ready(function () {
    // Get movie data from window object
    const movieId = window.movieData.id;
    const movieTitle = window.movieData.title;

    let player;
    let watchStartTime = 0;
    let totalWatchTime = 0;
    let isPlaying = false;
    let inactivityTimer;

    const infoBar = $('#infoBar');
    const controlsBar = $('#controlsBar');

    // Initialize Video.js Player
    initializePlayer();

    // Setup Auto-hide Controls
    setupAutoHideControls();

    // Setup Event Listeners
    setupEventListeners();

    // Setup Keyboard Shortcuts
    setupKeyboardShortcuts();

    // Resume from Last Position
    resumeFromLastPosition();

    // Save Position Periodically
    savePositionPeriodically();

    /**
     * Initialize Video.js player with configuration
     */
    function initializePlayer() {
        player = videojs('movie-player', {
            fluid: true,
            responsive: true,
            controls: true,
            preload: 'auto',
            playbackRates: [0.5, 0.75, 1, 1.25, 1.5, 2]
        });

        console.log('Video player initialized for movie:', movieTitle);
    }

    /**
     * Setup auto-hide functionality for controls
     */
    function setupAutoHideControls() {
        function resetInactivityTimer() {
            clearTimeout(inactivityTimer);
            showControls();

            if (isPlaying) {
                inactivityTimer = setTimeout(hideControls, 3000);
            }
        }

        function showControls() {
            infoBar.removeClass('hidden');
            controlsBar.removeClass('hidden');
        }

        function hideControls() {
            infoBar.addClass('hidden');
            controlsBar.addClass('hidden');
        }

        $(document).on('mousemove click keydown touchstart', resetInactivityTimer);
        resetInactivityTimer();
    }

    /**
     * Setup all player event listeners
     */
    function setupEventListeners() {
        // Play event
        player.on('play', function () {
            watchStartTime = Date.now();
            isPlaying = true;
            console.log('Playback started');
        });

        // Pause event
        player.on('pause', function () {
            isPlaying = false;
            recordWatchTime();
            clearTimeout(inactivityTimer);
            infoBar.removeClass('hidden');
            controlsBar.removeClass('hidden');
            console.log('Playback paused');
        });

        // Ended event
        player.on('ended', function () {
            isPlaying = false;
            recordWatchTime();
            showCompletionMessage();
            clearLastPosition();
            console.log('Playback ended');
        });

        // Exit fullscreen button
        $('#exitFullscreen').click(function () {
            // If in fullscreen, exit it
            if (document.fullscreenElement || document.webkitFullscreenElement || document.mozFullScreenElement || document.msFullscreenElement) {
                if (document.exitFullscreen) {
                    document.exitFullscreen();
                } else if (document.webkitExitFullscreen) {
                    document.webkitExitFullscreen();
                } else if (document.mozCancelFullScreen) {
                    document.mozCancelFullScreen();
                } else if (document.msExitFullscreen) {
                    document.msExitFullscreen();
                }
            }
            // If player is in fullscreen mode, exit it
            if (player.isFullscreen()) {
                player.exitFullscreen();
            }
            // Always navigate back to details page
            window.location.href = '/Movie/Details/' + movieId;
        });

        // Toggle info bar button
        $('#toggleInfo').click(function () {
            infoBar.toggleClass('hidden');
        });

        // Record watch time before leaving page
        $(window).on('beforeunload', function () {
            if (watchStartTime > 0) {
                recordWatchTime(false);
            }
        });
    }

    /**
     * Record watch time to server
     */
    function recordWatchTime(async = true) {
        if (watchStartTime === 0) return;

        const currentTime = Date.now();
        const duration = Math.floor((currentTime - watchStartTime) / 1000);
        totalWatchTime += duration;

        if (duration < 1) return; // Don't record less than 1 second

        console.log(`Recording ${duration} seconds of watch time`);

        $.ajax({
            url: '/Rental/RecordWatch',
            type: 'POST',
            async: async,
            data: {
                movieId: movieId,
                watchDurationSeconds: duration,
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                console.log('Watch time recorded successfully');
            },
            error: function (xhr, status, error) {
                console.error('Failed to record watch time:', error);
            }
        });

        watchStartTime = 0;
    }

    /**
     * Show completion message
     */
    function showCompletionMessage() {
        const minutes = Math.floor(totalWatchTime / 60);
        const message = minutes > 0
            ? `Thanks for watching! You watched for ${minutes} minute${minutes !== 1 ? 's' : ''}.`
            : 'Thanks for watching!';

        alert(message);
    }

    /**
     * Setup keyboard shortcuts
     */
    function setupKeyboardShortcuts() {
        $(document).keydown(function (e) {
            // Ignore if typing in input field
            if ($(e.target).is('input, textarea')) return;

            switch (e.key) {
                case 'f':
                case 'F':
                    // Toggle fullscreen
                    e.preventDefault();
                    if (player.isFullscreen()) {
                        player.exitFullscreen();
                    } else {
                        player.requestFullscreen();
                    }
                    break;

                case 'Escape':
                    // Exit fullscreen
                    if (player.isFullscreen()) {
                        player.exitFullscreen();
                    }
                    break;

                case ' ':
                    // Play/Pause
                    e.preventDefault();
                    if (player.paused()) {
                        player.play();
                    } else {
                        player.pause();
                    }
                    break;

                case 'ArrowRight':
                    // Skip forward 10 seconds
                    e.preventDefault();
                    player.currentTime(player.currentTime() + 10);
                    break;

                case 'ArrowLeft':
                    // Skip backward 10 seconds
                    e.preventDefault();
                    player.currentTime(player.currentTime() - 10);
                    break;

                case 'm':
                case 'M':
                    // Toggle mute
                    e.preventDefault();
                    player.muted(!player.muted());
                    break;
            }
        });
    }

    /**
     * Resume playback from last saved position
     */
    function resumeFromLastPosition() {
        const storageKey = `movie_${movieId}_position`;
        const lastPosition = localStorage.getItem(storageKey);

        if (lastPosition && parseFloat(lastPosition) > 5) {
            const position = parseFloat(lastPosition);
            const minutes = Math.floor(position / 60);
            const seconds = Math.floor(position % 60);

            if (confirm(`Resume from ${minutes}:${seconds.toString().padStart(2, '0')}?`)) {
                player.currentTime(position);
            }
        }
    }

    /**
     * Save current position periodically
     */
    function savePositionPeriodically() {
        setInterval(function () {
            if (!player.paused() && player.currentTime() > 0) {
                const storageKey = `movie_${movieId}_position`;
                localStorage.setItem(storageKey, player.currentTime().toString());
            }
        }, 10000); // Save every 10 seconds
    }

    /**
     * Clear last saved position
     */
    function clearLastPosition() {
        const storageKey = `movie_${movieId}_position`;
        localStorage.removeItem(storageKey);
    }
});