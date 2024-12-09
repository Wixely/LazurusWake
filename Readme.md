# LazurusWake

LazurusWake is a lightweight monitoring tool for Docker that ensures containers with the `LazurusWake` label are always running. If a container fails to start (e.g., due to dependencies like unavailable NFS remote storage), LazurusWake will continuously attempt to start it every minute until it successfully runs.

This tool is particularly useful in scenarios where Docker's `restart: always` policy is insufficient, such as when external dependencies like network-attached storage (NAS) or remote databases are not immediately available during startup.

---

## Features

- **Targeted Monitoring**: Only monitors Docker containers labeled with `LazurusWake`.
- **Automatic Recovery**: Attempts to start stopped containers every minute until they are running.
- **Lightweight**: Minimal resource usage, leveraging Docker's API directly.
- **Custom Use Cases**: Designed for situations where `restart: always` fails due to external dependencies.

---

## Use Cases

- **Delayed NAS Availability**: Containers dependent on NFS or other remote storage systems that may not be mounted during Docker startup.
- **Application Dependency**: Containers that require external services (e.g., databases, APIs) to be available before starting.
- **Custom Scenarios**: Any situation where a Docker container fails to start repeatedly due to environmental factors outside its control.

---

## How It Works

1. **Label Identification**:
   - Containers to be monitored must have the label `LazurusWake` set (e.g., `LazurusWake=true`).

2. **Continuous Monitoring**:
   - LazurusWake periodically checks for containers with the `LazurusWake` label that are in a stopped state (`exited`).

3. **Restart Attempts**:
   - For each stopped container, LazurusWake invokes the Docker API to attempt starting the container.
   - This process is repeated every minute until the container successfully starts.

---

## Installation

### Prerequisites

- Docker must be installed and running.
- The user running LazurusWake must have access to the Docker socket (`/var/run/docker.sock`).

### Running LazurusWake

Use the following Docker Compose configuration to deploy LazurusWake:

```yaml
version: '3.8'

services:
  lazuruswake:
    image: wixely/lazuruswake:latest
    container_name: lazuruswake
    volumes:
      - /var/run/docker.sock:/var/run/docker.sock
    restart: unless-stopped


### Docker Compose Label Example LazurusWake

Include a label like this to make it monitored by LazurusWake:

```yaml
version: '3.8'

services:
  lazuruswake:
    image: popular/dockerimage:latest
    container_name: genericimage
    labels:
      LazurusWake: "true"
