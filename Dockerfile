FROM golang:1.24-alpine AS builder

WORKDIR /app

COPY ./ ./

WORKDIR /app
	
RUN CGO_ENABLED=0 GOOS=linux go build -o /server

# Stage 2: Create a minimal image for the final executable
FROM alpine:3.22

# Set the working directory
WORKDIR /root/

# Copy the compiled binary from the builder stage
COPY --from=builder /server .

# Expose the port your Go server listens on
EXPOSE 8080

RUN apk -U add yt-dlp

RUN apk -U add yt-dlp-core

RUN apk -U upgrade yt-dlp

# Command to run the application
CMD ["./server"]
