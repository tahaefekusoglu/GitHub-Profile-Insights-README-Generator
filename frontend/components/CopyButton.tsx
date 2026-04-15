"use client";

import { useState } from "react";
import { Check, Copy } from "lucide-react";

interface Props {
  text: string | null;
}

export default function CopyButton({ text }: Props) {
  const [copied, setCopied] = useState(false);

  async function handleCopy() {
    if (!text) return;
    try {
      if (navigator.clipboard) {
        await navigator.clipboard.writeText(text);
      } else {
        const ta = document.createElement("textarea");
        ta.value = text;
        ta.style.position = "fixed";
        ta.style.opacity = "0";
        document.body.appendChild(ta);
        ta.focus();
        ta.select();
        document.execCommand("copy");
        document.body.removeChild(ta);
      }
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    } catch {
      alert("Failed to copy. Please select the text manually.");
    }
  }

  return (
    <div className="relative">
      <button
        onClick={handleCopy}
        disabled={!text}
        title={!text ? "Generate a README first" : "Copy to clipboard"}
        className={`flex items-center gap-2 px-5 py-3 rounded-xl font-semibold text-sm transition-all w-full justify-center ${
          text
            ? "bg-violet-600 hover:bg-violet-700 text-white cursor-pointer"
            : "bg-gray-100 dark:bg-gray-800 text-gray-400 cursor-not-allowed"
        }`}
      >
        {copied ? <Check size={16} /> : <Copy size={16} />}
        {copied ? "Copied!" : "Copy Markdown"}
      </button>

      {copied && (
        <div className="fixed bottom-6 right-6 bg-gray-900 text-white text-sm px-4 py-2 rounded-lg shadow-lg z-50 animate-fade-in">
          Copied to clipboard!
        </div>
      )}
    </div>
  );
}
