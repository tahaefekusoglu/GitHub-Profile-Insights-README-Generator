interface Props {
  markdown: string | null;
}

export default function ReadmePreview({ markdown }: Props) {
  if (!markdown) {
    return (
      <div className="rounded-xl border-2 border-dashed border-gray-200 dark:border-gray-700 h-64 flex flex-col items-center justify-center gap-2 text-center p-6">
        <p className="text-gray-400 dark:text-gray-500 text-sm font-medium">
          Your README will appear here
        </p>
        <p className="text-gray-300 dark:text-gray-600 text-xs">
          Configure your options on the left, then click &quot;Build README&quot;
        </p>
      </div>
    );
  }

  const charCount = markdown.length;
  const lineCount = markdown.split("\n").length;

  return (
    <div className="rounded-xl border border-gray-200 dark:border-gray-800 overflow-hidden">
      <pre className="bg-gray-950 text-gray-100 p-5 text-xs font-mono overflow-x-auto whitespace-pre-wrap max-h-[600px] overflow-y-auto leading-relaxed">
        <code>{markdown}</code>
      </pre>
      <div className="px-5 py-2 bg-gray-900 border-t border-gray-800">
        <p className="text-xs text-gray-500">
          {charCount.toLocaleString()} characters · {lineCount.toLocaleString()} lines
        </p>
      </div>
    </div>
  );
}
