# FileOrganizer
A simple C# console application that automatically organizes files into folders based on their extensions.

## Features

- Sorts files into categories:
  - Images
  - Videos
  - Documents
  - Audio
  - Programs
  - Archives
  - Other
- Preview mode before moving files.
- Shows processing progress.
- Displays statistics:
  - number of files in each category;
  - total size of each category;
  - total files processed;
  - moved and skipped files.
- Skips files that are already inside category folders.
- Prevents overwriting existing files.

## Usage

1. Run the application.
2. Enter the path to the folder.
3. Review the preview.
4. Confirm with `y` to organize the files.

## Example

Before:

```
Downloads/
    photo.jpg
    song.mp3
    archive.zip
```

After:

```
Downloads/
    Images/
        photo.jpg
    Audio/
        song.mp3
    Archives/
        archive.zip
```