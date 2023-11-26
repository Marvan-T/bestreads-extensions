[![CI workflow](https://github.com/Marvan-T/bestreads-extensions/actions/workflows/CI.yml/badge.svg)](https://github.com/Marvan-T/bestreads-extensions/actions/workflows/CI.yml)

# Best Reads Extensions

## Project Overview

"Best Reads" is a web application originally developed as a final year group project at university. Our aim is to enhance the book discovery experience for users.

- Original Project Repository: [Best Reads](https://github.com/laurenmaylittle-cs/book-recommendations)

## Current Version

I am actively maintaining and improving a fork of the original project:

- Fork Repository: [Best Reads Fork](https://github.com/Marvan-T/book-recommendations)

## Deployment

The project is deployed on Azure. You can visit the application using either of the following URLs:

- Main URL: [Best Reads](https://best-reads.live)
- Alternative URL: [Best Reads on Azure](https://best-reads.azurewebsites.net)

## Enhancements and Features

Best Reads Extensions is built as an extra layer on top of the original project. It works as its own API, exposing new features to the original application.

### New Features Introduced:

#### Recommendations

- Implemented a new recommendations layer utilizing embeddings to identify similar items (books).
- An overview of the process is illustrated in the workflow diagram below.

<img src="./Images/Flowchart-Recommendations.png" width="90%">


[![Thumbnail](https://github.com/path/to/thumbnail.jpg){: width="50%"}](https://github.com/Marvan-T/bestreads-extensions/assets/65969444/dd644f80-2b07-4452-baf3-350206d308ad)
