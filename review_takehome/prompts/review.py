import os
import json
from github import Github
from pathlib import Path
from openai import OpenAI
import difflib

# Initialize OpenAI client
client = OpenAI()

MAX_CHARS = 12000  # GPT-4 input safety buffer

def get_pr_context():
    repo_name = os.getenv("GITHUB_REPOSITORY")
    event_path = os.getenv("GITHUB_EVENT_PATH")
    with open(event_path, "r") as f:
        event = json.load(f)

    pr_number = event["number"]
    g = Github(os.getenv("GITHUB_TOKEN"))
    repo = g.get_repo(repo_name)
    pr = repo.get_pull(pr_number)
    changed_files = list(pr.get_files())

    return pr, changed_files

def load_instructions():
    return Path("review_takehome/prompts/instructions.md").read_text()

def clean_patch(patch):
    """
    Remove hunks that are simple deletions followed by identical additions (noise).
    """
    cleaned_lines = []
    old_lines = []
    new_lines = []

    for line in patch.splitlines():
        if line.startswith('-'):
            old_lines.append(line[1:].strip())
        elif line.startswith('+'):
            new_lines.append(line[1:].strip())
        else:
            old_lines.clear()
            new_lines.clear()
            cleaned_lines.append(line)

        # If both collected, compare
        if old_lines and new_lines and len(old_lines) == len(new_lines):
            if old_lines != new_lines:
                cleaned_lines.extend(f"- {l}" for l in old_lines)
                cleaned_lines.extend(f"+ {l}" for l in new_lines)
            old_lines.clear()
            new_lines.clear()

    return "\n".join(cleaned_lines)

def split_diff(diff, max_chars):
    """
    Split a diff string into chunks within max_chars limits
    """
    chunks = []
    lines = diff.splitlines()
    current = []

    for line in lines:
        current.append(line)
        if sum(len(l) for l in current) > max_chars:
            chunks.append("\n".join(current))
            current = []

    if current:
        chunks.append("\n".join(current))

    return chunks

def generate_file_review(instructions, filename, cleaned_patch):
    chunks = split_diff(cleaned_patch, MAX_CHARS)

    reviews = []
    for i, chunk in enumerate(chunks):
        context_note = f"(Part {i+1} of {len(chunks)})" if len(chunks) > 1 else ""
        prompt = f"""{instructions}

{context_note}

Review this Unity candidate submission file:

Filename: {filename}

Diff:
{chunk}
"""
        response = client.chat.completions.create(
            model="gpt-4",
            messages=[
                {"role": "system", "content": "You are a senior Unity engineering reviewer."},
                {"role": "user", "content": prompt}
            ],
            temperature=0.2,
        )
        reviews.append(response.choices[0].message.content)

    return reviews

def post_comment(pr, text):
    pr.create_issue_comment(text)

def main():
    pr, changed_files = get_pr_context()
    instructions = load_instructions()

    for file in changed_files:
        if not hasattr(file, "patch") or not file.patch:
            continue

        cleaned = clean_patch(file.patch)
        reviews = generate_file_review(instructions, file.filename, cleaned)

        for i, review in enumerate(reviews):
            title = f"### 📝 Review for `{file.filename}`"
            if len(reviews) > 1:
                title += f" (Part {i+1}/{len(reviews)})"
            post_comment(pr, f"{title}\n\n{review}")

if __name__ == "__main__":
    main()
