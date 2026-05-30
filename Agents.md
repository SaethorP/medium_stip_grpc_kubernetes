# Repository Instructions

## README Maintenance

- Keep `README.md` updated whenever commands, ports, environment variables, Docker usage, Helm usage, project structure, or test instructions change.
- Add runnable commands for new workflows instead of only describing them.
- If a new environment variable is introduced, document its default behavior and at least one example value.
- If Kubernetes or Helm templates change, update the README with the matching install or upgrade command.
- Before finishing a change, run the most relevant verification command and record the command in the final response.

## Project Notes

- `DebitCardApi` is the gRPC API project.
- `DebitCardApi.Tests` contains xUnit tests discoverable by Test Explorer.
- The API listens on HTTPS port `50051` with HTTP/2.
- `STIP_Enabled=true` selects the STIP business and data layer implementations. Any other value, including unset, selects the regular implementations.
