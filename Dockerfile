FROM golang:1.24-alpine AS builder

WORKDIR /app

COPY ./go.mod ./go.sum ./

COPY ./src ./server/

WORKDIR /app/server
	
RUN CGO_ENABLED=0 GOOS=linux go build -o /server

# Stage 2: Create a minimal image for the final executable
FROM ubuntu:latest

# Set the working directory
WORKDIR /root/

# Copy the compiled binary from the builder stage
COPY --from=builder /server .

# Expose the port your Go server listens on
EXPOSE 8080

RUN apt update

RUN apt install -y yt-dlp

RUN apt install -y ffmpeg

# Command to run the application
CMD ["./server"]

