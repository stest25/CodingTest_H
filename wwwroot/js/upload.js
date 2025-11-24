// Upload Manager - Handles file upload functionality
const UploadManager = {
    maxFileSize: 209715200, // 200MB
    loadingOverlay: null,
    progressBar: null,
    progressPercent: null,
    form: null,

    init() {
 this.loadingOverlay = document.getElementById('loadingOverlay');
    this.progressBar = document.getElementById('progressBar');
        this.progressPercent = document.getElementById('progressPercent');
        this.form = document.getElementById('uploadForm');

    if (!this.form) return;

        this.form.addEventListener('submit', (e) => this.handleSubmit(e));
   window.addEventListener('beforeunload', (e) => this.handleBeforeUnload(e));
    },

    showLoading() {
 if (!this.loadingOverlay) return;
        this.loadingOverlay.classList.add('active');
document.body.style.cursor = 'wait';
     document.body.style.pointerEvents = 'none';
        this.loadingOverlay.style.pointerEvents = 'auto';
    },

    hideLoading() {
      if (!this.loadingOverlay) return;
        this.loadingOverlay.classList.remove('active');
        document.body.style.cursor = 'default';
  document.body.style.pointerEvents = 'auto';
        if (this.progressBar) this.progressBar.style.width = '0%';
  if (this.progressPercent) this.progressPercent.innerText = '0%';
    },

    updateProgress(percent) {
        if (this.progressBar) {
        this.progressBar.style.width = percent + '%';
        }
        if (this.progressPercent) {
            this.progressPercent.innerText = Math.round(percent) + '%';
   }
    },

  async handleSubmit(e) {
        e.preventDefault();

        const fileInput = this.form.querySelector('input[type="file"]');
        const files = fileInput.files;
        const message = document.getElementById('message');

        if (!files.length) {
 message.innerText = 'Please select at least one file.';
            return;
        }

        const formData = new FormData();

        // Validate files
        for (const f of files) {
            if (f.size > this.maxFileSize) {
       message.innerText = `File ${f.name} exceeds the 200MB limit.`;
        return;
     }
            formData.append('files', f);
        }

 const button = this.form.querySelector('button');
        button.disabled = true;
        button.innerText = 'Uploading...';
        message.innerText = '';

    this.showLoading();

        try {
  await this.uploadFiles(formData, button, message);
        } catch (error) {
    this.hideLoading();
            button.disabled = false;
     button.innerText = 'Upload Files';
     message.innerText = 'Error: Unable to connect to the server.';
   }
    },

    uploadFiles(formData, button, message) {
        return new Promise((resolve, reject) => {
            const xhr = new XMLHttpRequest();

   // Track upload progress
       xhr.upload.addEventListener('progress', (e) => {
              if (e.lengthComputable) {
       const percentComplete = (e.loaded / e.total) * 100;
        this.updateProgress(percentComplete);
  }
            });

            // Handle completion
     xhr.addEventListener('load', () => {
      this.hideLoading();
     button.disabled = false;
         button.innerText = 'Upload Files';

       if (xhr.status === 200) {
 window.location.href = '/';
                } else if (xhr.status === 413) {
     message.innerText = 'Error: File size exceeds the 200MB limit.';
            } else {
          const error = xhr.responseText;
    message.innerText = `Error: ${error}`;
   }
        resolve();
       });

            // Handle errors
 xhr.addEventListener('error', () => {
      this.hideLoading();
          button.disabled = false;
       button.innerText = 'Upload Files';
       message.innerText = 'Error: Unable to connect to the server.';
        reject(new Error('Upload failed'));
  });

      // Handle abort
      xhr.addEventListener('abort', () => {
       this.hideLoading();
      button.disabled = false;
     button.innerText = 'Upload Files';
    message.innerText = 'Upload cancelled.';
       reject(new Error('Upload cancelled'));
   });

            // Send request
   xhr.open('POST', '/api/upload');
    xhr.send(formData);
        });
    },

    handleBeforeUnload(e) {
        if (this.loadingOverlay && this.loadingOverlay.classList.contains('active')) {
    e.preventDefault();
            e.returnValue = 'Upload in progress. Are you sure you want to leave?';
            return e.returnValue;
        }
    }
};

// Initialize when DOM is ready
if (document.readyState === 'loading') {
  document.addEventListener('DOMContentLoaded', () => UploadManager.init());
} else {
    UploadManager.init();
}
