import Image from "next/image";
import { GitHubProfile } from "@/lib/types";

interface Props {
  profile: GitHubProfile;
}

export default function ProfileCard({ profile }: Props) {
  return (
    <div className="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 p-5">
      <div className="flex items-center gap-4 mb-4">
        <Image
          src={profile.avatarUrl}
          alt={profile.name}
          width={64}
          height={64}
          className="rounded-full"
        />
        <div>
          <p className="font-bold text-lg text-gray-900 dark:text-gray-100">{profile.name}</p>
          <p className="text-sm text-gray-500 dark:text-gray-400">@{profile.login}</p>
        </div>
      </div>

      {profile.bio && (
        <p className="text-sm text-gray-600 dark:text-gray-300 mb-4">{profile.bio}</p>
      )}

      <div className="grid grid-cols-2 gap-2">
        {[
          { label: "Repos", value: profile.publicRepos },
          { label: "Stars", value: profile.totalStars },
          { label: "Followers", value: profile.followers },
          { label: "Following", value: profile.following },
        ].map((stat) => (
          <div
            key={stat.label}
            className="rounded-lg bg-gray-50 dark:bg-gray-800 px-3 py-2 text-center"
          >
            <p className="font-bold text-gray-900 dark:text-gray-100">{stat.value.toLocaleString()}</p>
            <p className="text-xs text-gray-500 dark:text-gray-400">{stat.label}</p>
          </div>
        ))}
      </div>
    </div>
  );
}
